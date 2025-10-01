using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementApplication.Interface.Services;
using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace HotelManagementInfratructure.Services
{
    public class BookingOtpService : IBookingOtpService
    {
        private readonly IBookingOtp _bookingOtpRepository;
        private readonly IBooking _bookingRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BookingOtpService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public BookingOtpService(
            IBookingOtp bookingOtpRepository,
            IBooking bookingRepository,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<BookingOtpService> logger,
            IUnitOfWork unitOfWork)
        {
            _bookingOtpRepository = bookingOtpRepository;
            _bookingRepository = bookingRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<BookingOtpDto>> GenerateOtpAsync(int bookingId, string email)
        {
            try
            {
                // Check if booking exists
                var booking = await _bookingRepository.GetById(bookingId);
                if (booking == null)
                {
                    return ResponseHelper.Failure<BookingOtpDto>("Booking not found", 404);
                }

                // Check if booking is in pending status
                if (booking.Status != BookingStatus.Pending)
                {
                    return ResponseHelper.Failure<BookingOtpDto>("Booking is not in pending status", 400);
                }

                // Check if there's already an active OTP
                if (await _bookingOtpRepository.HasActiveOtpAsync(bookingId))
                {
                    return ResponseHelper.Failure<BookingOtpDto>("An active OTP already exists for this booking", 400);
                }

                // Generate OTP
                var otpCode = GenerateOtpCode();
                var expiresAt = DateTime.UtcNow.AddMinutes(15); // OTP expires in 15 minutes

                // Create OTP record
                var bookingOtp = new BookingOtp
                {
                    BookingId = bookingId,
                    OtpCode = otpCode,
                    Email = email,
                    ExpiresAt = expiresAt,
                    Status = OtpStatus.Pending
                };

                await _bookingOtpRepository.Add(bookingOtp);
                await _unitOfWork.SaveAsync();

                // Send OTP email
                await SendOtpEmailAsync(booking, otpCode, email);

                var otpDto = new BookingOtpDto
                {
                    Id = bookingOtp.Id,
                    BookingId = bookingOtp.BookingId,
                    OtpCode = bookingOtp.OtpCode,
                    Email = bookingOtp.Email,
                    ExpiresAt = bookingOtp.ExpiresAt,
                    IsUsed = bookingOtp.IsUsed,
                    UsedAt = bookingOtp.UsedAt,
                    Status = bookingOtp.Status,
                    CreatedAt = bookingOtp.CreatedAt,
                    UpdatedAt = bookingOtp.UpdatedAt
                };

                _logger.LogInformation($"OTP generated for booking {bookingId}");
                return ResponseHelper.Success(otpDto, "OTP generated and sent successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating OTP for booking {bookingId}");
                return ResponseHelper.Failure<BookingOtpDto>($"Error generating OTP: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<BookingOtpResponseDto>> VerifyOtpAsync(int bookingId, string otpCode)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Check if booking exists with navigation properties
                var booking = await _bookingRepository.GetByIdWithNavigationPropertiesAsync(bookingId);
                if (booking == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ResponseHelper.Failure<BookingOtpResponseDto>("Booking not found", 404);
                }

                // Ensure navigation properties are loaded
                if (booking.Guest == null || booking.Room == null)
                {
                    _logger.LogError($"Booking {bookingId} is missing navigation properties (Guest or Room)");
                    await _unitOfWork.RollbackTransactionAsync();
                    return ResponseHelper.Failure<BookingOtpResponseDto>("Booking data is incomplete", 500);
                }

                // Check if booking is in pending status
                if (booking.Status != BookingStatus.Pending)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ResponseHelper.Failure<BookingOtpResponseDto>("Booking is not in pending status", 400);
                }

                // Verify OTP
                var isValid = await _bookingOtpRepository.IsOtpValidAsync(bookingId, otpCode);
                if (!isValid)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ResponseHelper.Failure<BookingOtpResponseDto>("Invalid or expired OTP", 400);
                }

                // Get OTP record
                var bookingOtp = await _bookingOtpRepository.GetByBookingIdAsync(bookingId);
                if (bookingOtp == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ResponseHelper.Failure<BookingOtpResponseDto>("OTP not found", 404);
                }

                // Mark OTP as used
                bookingOtp.IsUsed = true;
                bookingOtp.UsedAt = DateTime.UtcNow;
                bookingOtp.Status = OtpStatus.Verified;
                bookingOtp.UpdatedAt = DateTime.UtcNow;

                // Update booking status to confirmed
                booking.Status = BookingStatus.Confirmed;
                booking.UpdatedAt = DateTime.UtcNow;

                await _bookingOtpRepository.Update(bookingOtp);
                await _bookingRepository.Update(booking);
                await _unitOfWork.CommitTransactionAsync();

                // Send booking confirmation email (after successful transaction)
                try
                {
                    await SendBookingConfirmationEmailAsync(booking);
                }
                catch (Exception emailEx)
                {
                    // Log the error but don't fail the OTP verification
                    _logger.LogError(emailEx, $"Failed to send booking confirmation email for booking {bookingId}");
                }

                var response = new BookingOtpResponseDto
                {
                    Success = true,
                    Message = "OTP verified successfully. Booking confirmed.",
                    BookingId = bookingId,
                    BookingStatus = booking.Status,
                    VerifiedAt = bookingOtp.UsedAt
                };

                _logger.LogInformation($"OTP verified for booking {bookingId}");
                return ResponseHelper.Success(response, "OTP verified successfully", 200);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error verifying OTP for booking {bookingId}");
                return ResponseHelper.Failure<BookingOtpResponseDto>($"Error verifying OTP: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<BookingOtpDto>> GetOtpByBookingIdAsync(int bookingId)
        {
            try
            {
                var bookingOtp = await _bookingOtpRepository.GetByBookingIdAsync(bookingId);
                if (bookingOtp == null)
                {
                    return ResponseHelper.Failure<BookingOtpDto>("OTP not found", 404);
                }

                var otpDto = new BookingOtpDto
                {
                    Id = bookingOtp.Id,
                    BookingId = bookingOtp.BookingId,
                    OtpCode = bookingOtp.OtpCode,
                    Email = bookingOtp.Email,
                    ExpiresAt = bookingOtp.ExpiresAt,
                    IsUsed = bookingOtp.IsUsed,
                    UsedAt = bookingOtp.UsedAt,
                    Status = bookingOtp.Status,
                    CreatedAt = bookingOtp.CreatedAt,
                    UpdatedAt = bookingOtp.UpdatedAt
                };

                return ResponseHelper.Success(otpDto, "OTP retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving OTP for booking {bookingId}");
                return ResponseHelper.Failure<BookingOtpDto>($"Error retrieving OTP: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> ResendOtpAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetById(bookingId);
                if (booking == null)
                {
                    return ResponseHelper.Failure<bool>("Booking not found", 404);
                }

                // Expire existing OTP
                await ExpireOtpAsync(bookingId);

                // Generate new OTP
                var result = await GenerateOtpAsync(bookingId, booking.Guest.Email);
                if (!result.Success)
                {
                    return ResponseHelper.Failure<bool>(result.Message, result.StatusCode);
                }

                return ResponseHelper.Success(true, "OTP resent successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resending OTP for booking {bookingId}");
                return ResponseHelper.Failure<bool>($"Error resending OTP: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> ExpireOtpAsync(int bookingId)
        {
            try
            {
                var bookingOtp = await _bookingOtpRepository.GetByBookingIdAsync(bookingId);
                if (bookingOtp != null && !bookingOtp.IsUsed)
                {
                    bookingOtp.Status = OtpStatus.Expired;
                    bookingOtp.UpdatedAt = DateTime.UtcNow;
                    await _bookingOtpRepository.Update(bookingOtp);
                    await _unitOfWork.SaveAsync();
                }

                return ResponseHelper.Success(true, "OTP expired successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error expiring OTP for booking {bookingId}");
                return ResponseHelper.Failure<bool>($"Error expiring OTP: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<BookingOtpDto>>> GetExpiredOtpsAsync()
        {
            try
            {
                var expiredOtps = await _bookingOtpRepository.GetExpiredOtpsAsync();
                var otpDtos = expiredOtps.Select(otp => new BookingOtpDto
                {
                    Id = otp.Id,
                    BookingId = otp.BookingId,
                    OtpCode = otp.OtpCode,
                    Email = otp.Email,
                    ExpiresAt = otp.ExpiresAt,
                    IsUsed = otp.IsUsed,
                    UsedAt = otp.UsedAt,
                    Status = otp.Status,
                    CreatedAt = otp.CreatedAt,
                    UpdatedAt = otp.UpdatedAt
                });

                return ResponseHelper.Success(otpDtos, "Expired OTPs retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expired OTPs");
                return ResponseHelper.Failure<IEnumerable<BookingOtpDto>>($"Error retrieving expired OTPs: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> CleanupExpiredOtpsAsync()
        {
            try
            {
                var deletedCount = await _bookingOtpRepository.DeleteExpiredOtpsAsync();
                _logger.LogInformation($"Cleaned up {deletedCount} expired OTPs");
                return ResponseHelper.Success(true, $"Cleaned up {deletedCount} expired OTPs", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired OTPs");
                return ResponseHelper.Failure<bool>($"Error cleaning up expired OTPs: {ex.Message}", 500);
            }
        }

        private string GenerateOtpCode()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomNumber = BitConverter.ToUInt32(bytes, 0);
            return (randomNumber % 1000000).ToString("D6"); // 6-digit OTP
        }

        private async Task SendOtpEmailAsync(Booking booking, string otpCode, string email)
        {
            try
            {
                var hotelName = _configuration["Email:HotelName"] ?? "Marvin's Hotel";
                var hotelAddress = _configuration["Email:HotelAddress"] ?? "Outer Space";
                var hotelPhone = _configuration["Email:HotelPhone"] ?? "+1-000-000-0000";

                var emailBody = $@"
<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #2c3e50; margin-bottom: 10px;'>{hotelName}</h1>
            <p style='color: #7f8c8d; margin: 0;'>{hotelAddress}</p>
        </div>
        
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px;'>
            <h2 style='color: #2c3e50; margin-top: 0;'>Booking Confirmation Required</h2>
            <p>Dear {booking.Guest.FirstName} {booking.Guest.LastName},</p>
            <p>Thank you for choosing {hotelName}. To confirm your booking, please use the OTP code below:</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <div style='background-color: #3498db; color: white; padding: 15px 30px; border-radius: 8px; display: inline-block;'>
                    <h1 style='margin: 0; font-size: 32px; letter-spacing: 5px;'>{otpCode}</h1>
                </div>
            </div>
            
            <p><strong>Booking Details:</strong></p>
            <ul style='list-style: none; padding: 0;'>
                <li style='margin-bottom: 8px;'><strong>Booking ID:</strong> {booking.BookingNumber}</li>
                <li style='margin-bottom: 8px;'><strong>Booking Status:</strong> <span style='color: #e74c3c;'>Pending</span></li>
                <li style='margin-bottom: 8px;'><strong>Room:</strong> {booking.Room.Number} - {booking.Room.Type}</li>
                <li style='margin-bottom: 8px;'><strong>Check-in:</strong> {booking.CheckIn:dddd, MMMM dd, yyyy}</li>
                <li style='margin-bottom: 8px;'><strong>Check-out:</strong> {booking.CheckOut:dddd, MMMM dd, yyyy}</li>
                <li style='margin-bottom: 8px;'><strong>Guests:</strong> {booking.Guests}</li>
                <li style='margin-bottom: 8px;'><strong>Total Amount:</strong> ${booking.TotalAmount:F2}</li>
            </ul>
            
            <p style='color: #e74c3c; font-weight: bold;'>⚠️ This OTP will expire in 15 minutes.</p>
            <p>Please enter this code in the booking confirmation page to complete your reservation.</p>
        </div>
        
        <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #ecf0f1;'>
            <p style='color: #7f8c8d; font-size: 14px; margin: 0;'>
                {hotelName}<br>
                {hotelAddress}<br>
                Phone: {hotelPhone}
            </p>
        </div>
    </div>
</body>
</html>";

                var emailDto = new EmailDto
                {
                    To = email,
                    Subject = $"Booking Confirmation - OTP Code - {booking.BookingNumber}",
                    Body = emailBody,
                    IsHtml = true
                };

                await _emailService.SendEmailAsync(emailDto);
                _logger.LogInformation($"OTP email sent to {email} for booking {booking.BookingNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending OTP email to {email}");
                throw;
            }
        }

        private async Task SendBookingConfirmationEmailAsync(Booking booking)
        {
            try
            {
                // Validate booking object and its navigation properties
                if (booking == null)
                {
                    _logger.LogError("Booking object is null in SendBookingConfirmationEmailAsync");
                    return;
                }

                if (booking.Guest == null)
                {
                    _logger.LogError($"Guest navigation property is null for booking {booking.Id}");
                    return;
                }

                if (booking.Room == null)
                {
                    _logger.LogError($"Room navigation property is null for booking {booking.Id}");
                    return;
                }

                var hotelName = _configuration["Email:HotelName"] ?? "Marvin's Hotel";
                var hotelAddress = _configuration["Email:HotelAddress"] ?? "Outer Space";
                var hotelPhone = _configuration["Email:HotelPhone"] ?? "+1-000-000-0000";

                var emailBody = $@"
<html>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #2c3e50; margin-bottom: 10px;'>{hotelName}</h1>
            <p style='color: #7f8c8d; margin: 0;'>{hotelAddress}</p>
        </div>
        
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin-bottom: 20px;'>
            <div style='text-align: center; margin-bottom: 20px;'>
                <div style='background-color: #27ae60; color: white; padding: 15px 30px; border-radius: 8px; display: inline-block;'>
                    <h2 style='margin: 0; color: white;'>✅ Booking Confirmed!</h2>
                </div>
            </div>
            
            <p>Dear {booking.Guest.FirstName} {booking.Guest.LastName},</p>
            <p>Congratulations! Your booking has been successfully confirmed. We're excited to welcome you to {hotelName}.</p>
            
            <div style='background-color: white; padding: 20px; border-radius: 8px; border-left: 4px solid #27ae60; margin: 20px 0;'>
                <h3 style='color: #2c3e50; margin-top: 0;'>Booking Details</h3>
                <ul style='list-style: none; padding: 0;'>
                    <li style='margin-bottom: 10px; padding: 8px 0; border-bottom: 1px solid #ecf0f1;'>
                        <strong>Booking ID:</strong> <span style='color: #3498db;'>{booking.BookingNumber}</span>
                    </li>
                    <li style='margin-bottom: 10px; padding: 8px 0; border-bottom: 1px solid #ecf0f1;'>
                        <strong>Booking Status:</strong> <span style='color: #27ae60; font-weight: bold;'>Confirmed</span>
                    </li>
                    <li style='margin-bottom: 10px; padding: 8px 0; border-bottom: 1px solid #ecf0f1;'>
                        <strong>Room:</strong> {booking.Room.Number} - {booking.Room.Type}
                    </li>
                    <li style='margin-bottom: 10px; padding: 8px 0; border-bottom: 1px solid #ecf0f1;'>
                        <strong>Check-in:</strong> {booking.CheckIn:dddd, MMMM dd, yyyy}
                    </li>
                    <li style='margin-bottom: 10px; padding: 8px 0; border-bottom: 1px solid #ecf0f1;'>
                        <strong>Check-out:</strong> {booking.CheckOut:dddd, MMMM dd, yyyy}
                    </li>
                    <li style='margin-bottom: 10px; padding: 8px 0; border-bottom: 1px solid #ecf0f1;'>
                        <strong>Guests:</strong> {booking.Guests}
                    </li>
                    <li style='margin-bottom: 10px; padding: 8px 0; border-bottom: 1px solid #ecf0f1;'>
                        <strong>Total Amount:</strong> <span style='color: #e74c3c; font-weight: bold;'>${booking.TotalAmount:F2}</span>
                    </li>
                    {(string.IsNullOrEmpty(booking.SpecialRequests) ? "" : $@"
                    <li style='margin-bottom: 10px; padding: 8px 0;'>
                        <strong>Special Requests:</strong> {booking.SpecialRequests}
                    </li>")}
                </ul>
            </div>
            
            <div style='background-color: #e8f5e8; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                <h4 style='color: #27ae60; margin-top: 0;'>🎉 What's Next?</h4>
                <ul style='margin: 0; padding-left: 20px;'>
                    <li>Keep this email as your booking confirmation</li>
                    <li>Arrive at the hotel on your check-in date</li>
                    <li>Present your ID and booking reference at reception</li>
                    <li>Enjoy your stay with us!</li>
                </ul>
            </div>
            
            <div style='background-color: #fff3cd; padding: 15px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #ffc107;'>
                <h4 style='color: #856404; margin-top: 0;'>📞 Need Help?</h4>
                <p style='margin: 0; color: #856404;'>
                    If you have any questions or need to modify your booking, please contact us at <strong>{hotelPhone}</strong> 
                    or reply to this email. We're here to help!
                </p>
            </div>
        </div>
        
        <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #ecf0f1;'>
            <p style='color: #7f8c8d; font-size: 14px; margin: 0;'>
                Thank you for choosing {hotelName}!<br>
                {hotelAddress}<br>
                Phone: {hotelPhone}
            </p>
            <p style='color: #95a5a6; font-size: 12px; margin: 10px 0 0 0;'>
                This is an automated confirmation email. Please do not reply to this message.
            </p>
        </div>
    </div>
</body>
</html>";

                var emailDto = new EmailDto
                {
                    To = booking.Guest.Email,
                    Subject = $"Booking Confirmed - {booking.BookingNumber} - {hotelName}",
                    Body = emailBody,
                    IsHtml = true
                };

                await _emailService.SendEmailAsync(emailDto);
                _logger.LogInformation($"Booking confirmation email sent to {booking.Guest.Email} for booking {booking.BookingNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending booking confirmation email for booking {booking.BookingNumber}");
                throw;
            }
        }
    }
}
