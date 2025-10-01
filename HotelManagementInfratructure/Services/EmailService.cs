using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HotelManagementInfratructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<EmailResponseDto> SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                // Validate email addresses
                var validationResult = ValidateEmailAddresses(emailDto);
                if (!validationResult.IsValid)
                {
                    return new EmailResponseDto
                    {
                        Success = false,
                        Message = "Invalid email addresses",
                        ErrorDetails = validationResult.ErrorMessage,
                        SentAt = DateTime.UtcNow
                    };
                }

                var message = CreateMimeMessage(emailDto);
                await SendMessageAsync(message);
                
                _logger.LogInformation($"Email sent successfully to {emailDto.To}");
                return new EmailResponseDto
                {
                    Success = true,
                    Message = "Email sent successfully",
                    SentAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {emailDto.To}");
                return new EmailResponseDto
                {
                    Success = false,
                    Message = "Failed to send email",
                    ErrorDetails = GetDetailedErrorMessage(ex),
                    SentAt = DateTime.UtcNow
                };
            }
        }

        public async Task<EmailResponseDto> SendTemplateEmailAsync(EmailTemplateDto templateDto)
        {
            try
            {
                var emailDto = await GenerateEmailFromTemplate(templateDto);
                return await SendEmailAsync(emailDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send template email to {templateDto.To}");
                return new EmailResponseDto
                {
                    Success = false,
                    Message = "Failed to send template email",
                    ErrorDetails = ex.Message,
                    SentAt = DateTime.UtcNow
                };
            }
        }

        public async Task<EmailResponseDto> SendBookingConfirmationAsync(BookingEmailData bookingData)
        {
            var templateDto = new EmailTemplateDto
            {
                To = bookingData.GuestEmail,
                TemplateType = EmailTemplateType.BookingConfirmation,
                TemplateData = ConvertBookingDataToDictionary(bookingData)
            };
            return await SendTemplateEmailAsync(templateDto);
        }

        public async Task<EmailResponseDto> SendBookingCancellationAsync(BookingEmailData bookingData)
        {
            var templateDto = new EmailTemplateDto
            {
                To = bookingData.GuestEmail,
                TemplateType = EmailTemplateType.BookingCancellation,
                TemplateData = ConvertBookingDataToDictionary(bookingData)
            };
            return await SendTemplateEmailAsync(templateDto);
        }

        public async Task<EmailResponseDto> SendBookingReminderAsync(BookingEmailData bookingData)
        {
            var templateDto = new EmailTemplateDto
            {
                To = bookingData.GuestEmail,
                TemplateType = EmailTemplateType.BookingReminder,
                TemplateData = ConvertBookingDataToDictionary(bookingData)
            };
            return await SendTemplateEmailAsync(templateDto);
        }

        public async Task<EmailResponseDto> SendCheckInReminderAsync(BookingEmailData bookingData)
        {
            var templateDto = new EmailTemplateDto
            {
                To = bookingData.GuestEmail,
                TemplateType = EmailTemplateType.CheckInReminder,
                TemplateData = ConvertBookingDataToDictionary(bookingData)
            };
            return await SendTemplateEmailAsync(templateDto);
        }

        public async Task<EmailResponseDto> SendCheckOutReminderAsync(BookingEmailData bookingData)
        {
            var templateDto = new EmailTemplateDto
            {
                To = bookingData.GuestEmail,
                TemplateType = EmailTemplateType.CheckOutReminder,
                TemplateData = ConvertBookingDataToDictionary(bookingData)
            };
            return await SendTemplateEmailAsync(templateDto);
        }

        public async Task<EmailResponseDto> SendMaintenanceNotificationAsync(MaintenanceEmailData maintenanceData)
        {
            var templateDto = new EmailTemplateDto
            {
                To = _configuration["Email:AdminEmail"] ?? "admin@hotel.com",
                TemplateType = EmailTemplateType.MaintenanceNotification,
                TemplateData = ConvertMaintenanceDataToDictionary(maintenanceData)
            };
            return await SendTemplateEmailAsync(templateDto);
        }

        public async Task<EmailResponseDto> SendWelcomeEmailAsync(string to, string userName)
        {
            var templateDto = new EmailTemplateDto
            {
                To = to,
                TemplateType = EmailTemplateType.WelcomeEmail,
                TemplateData = new Dictionary<string, object>
                {
                    { "UserName", userName },
                    { "HotelName", _configuration["Email:HotelName"] ?? "Hotel Management System" }
                }
            };
            return await SendTemplateEmailAsync(templateDto);
        }

        public async Task<EmailResponseDto> SendPasswordResetEmailAsync(PasswordResetEmailData resetData)
        {
            var templateDto = new EmailTemplateDto
            {
                To = resetData.UserName,
                TemplateType = EmailTemplateType.PasswordReset,
                TemplateData = ConvertPasswordResetDataToDictionary(resetData)
            };
            return await SendTemplateEmailAsync(templateDto);
        }

        public async Task<EmailResponseDto> SendInvoiceEmailAsync(BookingEmailData bookingData, byte[] invoicePdf)
        {
            var templateDto = new EmailTemplateDto
            {
                To = bookingData.GuestEmail,
                TemplateType = EmailTemplateType.Invoice,
                TemplateData = ConvertBookingDataToDictionary(bookingData),
                Attachments = new List<EmailAttachmentDto>
                {
                    new EmailAttachmentDto
                    {
                        FileName = $"Invoice_{bookingData.BookingNumber}.pdf",
                        Content = invoicePdf,
                        ContentType = "application/pdf"
                    }
                }
            };
            return await SendTemplateEmailAsync(templateDto);
        }

        public async Task<bool> ValidateEmailConfigurationAsync()
        {
            try
            {
                using var client = new SmtpClient();
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    return false;
                }

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email configuration validation failed");
                return false;
            }
        }

        public async Task<List<EmailResponseDto>> SendBulkEmailsAsync(List<EmailDto> emailDtos)
        {
            var results = new List<EmailResponseDto>();
            
            foreach (var emailDto in emailDtos)
            {
                var result = await SendEmailAsync(emailDto);
                results.Add(result);
                
                // Add a small delay between emails to avoid overwhelming the SMTP server
                await Task.Delay(100);
            }
            
            return results;
        }

        private MimeMessage CreateMimeMessage(EmailDto emailDto)
        {
            var message = new MimeMessage();
            
            try
            {
                // From
                var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@hotel.com";
                var fromName = _configuration["Email:FromName"] ?? "Hotel Management System";
                
                // Validate and parse From address
                if (!IsValidEmailAddress(fromEmail))
                {
                    throw new ArgumentException($"Invalid From email address: {fromEmail}");
                }
                message.From.Add(new MailboxAddress(fromName, fromEmail));
                
                // To
                if (!IsValidEmailAddress(emailDto.To))
                {
                    throw new ArgumentException($"Invalid To email address: {emailDto.To}");
                }
                message.To.Add(MailboxAddress.Parse(emailDto.To));
                
                // CC
                if (!string.IsNullOrEmpty(emailDto.Cc))
                {
                    if (!IsValidEmailAddress(emailDto.Cc))
                    {
                        throw new ArgumentException($"Invalid CC email address: {emailDto.Cc}");
                    }
                    message.Cc.Add(MailboxAddress.Parse(emailDto.Cc));
                }
                
                // BCC
                if (!string.IsNullOrEmpty(emailDto.Bcc))
                {
                    if (!IsValidEmailAddress(emailDto.Bcc))
                    {
                        throw new ArgumentException($"Invalid BCC email address: {emailDto.Bcc}");
                    }
                    message.Bcc.Add(MailboxAddress.Parse(emailDto.Bcc));
                }
                
                // Subject
                message.Subject = emailDto.Subject ?? "No Subject";
                
                // Body
                var bodyBuilder = new BodyBuilder();
                if (emailDto.IsHtml)
                {
                    bodyBuilder.HtmlBody = emailDto.Body ?? "";
                }
                else
                {
                    bodyBuilder.TextBody = emailDto.Body ?? "";
                }
                
                // Attachments
                if (emailDto.Attachments != null)
                {
                    foreach (var attachment in emailDto.Attachments)
                    {
                        if (attachment.Content != null && attachment.Content.Length > 0)
                        {
                            bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                        }
                    }
                }
                
                message.Body = bodyBuilder.ToMessageBody();
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating MIME message");
                throw new InvalidOperationException($"Failed to create email message: {ex.Message}", ex);
            }
        }

        private async Task SendMessageAsync(MimeMessage message)
        {
            using var client = new SmtpClient();
            
            var smtpHost = _configuration["Email:SmtpHost"] ?? throw new InvalidOperationException("SMTP host not configured");
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUsername = _configuration["Email:SmtpUsername"] ?? throw new InvalidOperationException("SMTP username not configured");
            var smtpPassword = _configuration["Email:SmtpPassword"] ?? throw new InvalidOperationException("SMTP password not configured");
            var useSsl = bool.Parse(_configuration["Email:UseSsl"] ?? "false");
            
            await client.ConnectAsync(smtpHost, smtpPort, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpUsername, smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private async Task<EmailDto> GenerateEmailFromTemplate(EmailTemplateDto templateDto)
        {
            var subject = GetEmailSubject(templateDto.TemplateType);
            var body = await GetEmailBody(templateDto.TemplateType, templateDto.TemplateData);
            
            return new EmailDto
            {
                To = templateDto.To,
                Cc = templateDto.Cc,
                Bcc = templateDto.Bcc,
                Subject = subject,
                Body = body,
                IsHtml = true,
                Attachments = templateDto.Attachments
            };
        }

        private string GetEmailSubject(EmailTemplateType templateType)
        {
            return templateType switch
            {
                EmailTemplateType.BookingConfirmation => "Booking Confirmation - Hotel Management System",
                EmailTemplateType.BookingCancellation => "Booking Cancellation - Hotel Management System",
                EmailTemplateType.BookingReminder => "Booking Reminder - Hotel Management System",
                EmailTemplateType.CheckInReminder => "Check-in Reminder - Hotel Management System",
                EmailTemplateType.CheckOutReminder => "Check-out Reminder - Hotel Management System",
                EmailTemplateType.MaintenanceNotification => "Maintenance Notification - Hotel Management System",
                EmailTemplateType.WelcomeEmail => "Welcome to Hotel Management System",
                EmailTemplateType.PasswordReset => "Password Reset - Hotel Management System",
                EmailTemplateType.Invoice => "Invoice - Hotel Management System",
                _ => "Notification from Hotel Management System"
            };
        }

        private async Task<string> GetEmailBody(EmailTemplateType templateType, Dictionary<string, object> templateData)
        {
            // In a real application, you would load these templates from files or a database
            // For now, I'll create simple HTML templates
            return templateType switch
            {
                EmailTemplateType.BookingConfirmation => GenerateBookingConfirmationTemplate(templateData),
                EmailTemplateType.BookingCancellation => GenerateBookingCancellationTemplate(templateData),
                EmailTemplateType.BookingReminder => GenerateBookingReminderTemplate(templateData),
                EmailTemplateType.CheckInReminder => GenerateCheckInReminderTemplate(templateData),
                EmailTemplateType.CheckOutReminder => GenerateCheckOutReminderTemplate(templateData),
                EmailTemplateType.MaintenanceNotification => GenerateMaintenanceNotificationTemplate(templateData),
                EmailTemplateType.WelcomeEmail => GenerateWelcomeEmailTemplate(templateData),
                EmailTemplateType.PasswordReset => GeneratePasswordResetTemplate(templateData),
                EmailTemplateType.Invoice => GenerateInvoiceTemplate(templateData),
                _ => GenerateDefaultTemplate(templateData)
            };
        }

        private string GenerateBookingConfirmationTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Booking Confirmation</h2>
                    <p>Dear {data.GetValueOrDefault("GuestName", "Guest")},</p>
                    <p>Your booking has been confirmed. Here are the details:</p>
                    <ul>
                        <li><strong>Booking Number:</strong> {data.GetValueOrDefault("BookingNumber", "N/A")}</li>
                        <li><strong>Room:</strong> {data.GetValueOrDefault("RoomNumber", "N/A")} - {data.GetValueOrDefault("RoomType", "N/A")}</li>
                        <li><strong>Check-in:</strong> {data.GetValueOrDefault("CheckIn", "N/A")}</li>
                        <li><strong>Check-out:</strong> {data.GetValueOrDefault("CheckOut", "N/A")}</li>
                        <li><strong>Total Amount:</strong> ${data.GetValueOrDefault("TotalAmount", "0")}</li>
                    </ul>
                    <p>We look forward to welcoming you!</p>
                    <p>Best regards,<br>{data.GetValueOrDefault("HotelName", "Hotel Management System")}</p>
                </body>
                </html>";
        }

        private string GenerateBookingCancellationTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Booking Cancellation</h2>
                    <p>Dear {data.GetValueOrDefault("GuestName", "Guest")},</p>
                    <p>Your booking has been cancelled. Here are the details:</p>
                    <ul>
                        <li><strong>Booking Number:</strong> {data.GetValueOrDefault("BookingNumber", "N/A")}</li>
                        <li><strong>Room:</strong> {data.GetValueOrDefault("RoomNumber", "N/A")}</li>
                        <li><strong>Check-in:</strong> {data.GetValueOrDefault("CheckIn", "N/A")}</li>
                        <li><strong>Check-out:</strong> {data.GetValueOrDefault("CheckOut", "N/A")}</li>
                    </ul>
                    <p>If you have any questions, please contact us.</p>
                    <p>Best regards,<br>{data.GetValueOrDefault("HotelName", "Hotel Management System")}</p>
                </body>
                </html>";
        }

        private string GenerateBookingReminderTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Booking Reminder</h2>
                    <p>Dear {data.GetValueOrDefault("GuestName", "Guest")},</p>
                    <p>This is a reminder about your upcoming booking:</p>
                    <ul>
                        <li><strong>Booking Number:</strong> {data.GetValueOrDefault("BookingNumber", "N/A")}</li>
                        <li><strong>Room:</strong> {data.GetValueOrDefault("RoomNumber", "N/A")}</li>
                        <li><strong>Check-in:</strong> {data.GetValueOrDefault("CheckIn", "N/A")}</li>
                        <li><strong>Check-out:</strong> {data.GetValueOrDefault("CheckOut", "N/A")}</li>
                    </ul>
                    <p>We look forward to seeing you soon!</p>
                    <p>Best regards,<br>{data.GetValueOrDefault("HotelName", "Hotel Management System")}</p>
                </body>
                </html>";
        }

        private string GenerateCheckInReminderTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Check-in Reminder</h2>
                    <p>Dear {data.GetValueOrDefault("GuestName", "Guest")},</p>
                    <p>Your check-in is tomorrow! Here are the details:</p>
                    <ul>
                        <li><strong>Booking Number:</strong> {data.GetValueOrDefault("BookingNumber", "N/A")}</li>
                        <li><strong>Room:</strong> {data.GetValueOrDefault("RoomNumber", "N/A")}</li>
                        <li><strong>Check-in:</strong> {data.GetValueOrDefault("CheckIn", "N/A")}</li>
                    </ul>
                    <p>Please arrive at the hotel reception for check-in.</p>
                    <p>Best regards,<br>{data.GetValueOrDefault("HotelName", "Hotel Management System")}</p>
                </body>
                </html>";
        }

        private string GenerateCheckOutReminderTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Check-out Reminder</h2>
                    <p>Dear {data.GetValueOrDefault("GuestName", "Guest")},</p>
                    <p>Your check-out is tomorrow. Here are the details:</p>
                    <ul>
                        <li><strong>Booking Number:</strong> {data.GetValueOrDefault("BookingNumber", "N/A")}</li>
                        <li><strong>Room:</strong> {data.GetValueOrDefault("RoomNumber", "N/A")}</li>
                        <li><strong>Check-out:</strong> {data.GetValueOrDefault("CheckOut", "N/A")}</li>
                    </ul>
                    <p>Please ensure you have packed all your belongings and return your room key.</p>
                    <p>Best regards,<br>{data.GetValueOrDefault("HotelName", "Hotel Management System")}</p>
                </body>
                </html>";
        }

        private string GenerateMaintenanceNotificationTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Maintenance Notification</h2>
                    <p>A maintenance task has been scheduled:</p>
                    <ul>
                        <li><strong>Room:</strong> {data.GetValueOrDefault("RoomNumber", "N/A")}</li>
                        <li><strong>Type:</strong> {data.GetValueOrDefault("MaintenanceType", "N/A")}</li>
                        <li><strong>Scheduled Date:</strong> {data.GetValueOrDefault("ScheduledDate", "N/A")}</li>
                        <li><strong>Description:</strong> {data.GetValueOrDefault("Description", "N/A")}</li>
                    </ul>
                    <p>Notes: {data.GetValueOrDefault("Notes", "None")}</p>
                    <p>Best regards,<br>{data.GetValueOrDefault("HotelName", "Hotel Management System")}</p>
                </body>
                </html>";
        }

        private string GenerateWelcomeEmailTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Welcome!</h2>
                    <p>Dear {data.GetValueOrDefault("UserName", "User")},</p>
                    <p>Welcome to {data.GetValueOrDefault("HotelName", "Hotel Management System")}!</p>
                    <p>We're excited to have you as part of our community.</p>
                    <p>Best regards,<br>{data.GetValueOrDefault("HotelName", "Hotel Management System")}</p>
                </body>
                </html>";
        }

        private string GeneratePasswordResetTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Password Reset</h2>
                    <p>Dear {data.GetValueOrDefault("UserName", "User")},</p>
                    <p>You requested a password reset. Click the link below to reset your password:</p>
                    <p><a href='{data.GetValueOrDefault("ResetUrl", "#")}'>Reset Password</a></p>
                    <p>This link will expire on {data.GetValueOrDefault("ExpiryTime", "N/A")}.</p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <p>Best regards,<br>Hotel Management System</p>
                </body>
                </html>";
        }

        private string GenerateInvoiceTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Invoice</h2>
                    <p>Dear {data.GetValueOrDefault("GuestName", "Guest")},</p>
                    <p>Please find attached your invoice for the following booking:</p>
                    <ul>
                        <li><strong>Booking Number:</strong> {data.GetValueOrDefault("BookingNumber", "N/A")}</li>
                        <li><strong>Room:</strong> {data.GetValueOrDefault("RoomNumber", "N/A")}</li>
                        <li><strong>Check-in:</strong> {data.GetValueOrDefault("CheckIn", "N/A")}</li>
                        <li><strong>Check-out:</strong> {data.GetValueOrDefault("CheckOut", "N/A")}</li>
                        <li><strong>Total Amount:</strong> ${data.GetValueOrDefault("TotalAmount", "0")}</li>
                    </ul>
                    <p>Best regards,<br>{data.GetValueOrDefault("HotelName", "Hotel Management System")}</p>
                </body>
                </html>";
        }

        private string GenerateDefaultTemplate(Dictionary<string, object> data)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Notification</h2>
                    <p>This is a notification from Hotel Management System.</p>
                    <p>Best regards,<br>Hotel Management System</p>
                </body>
                </html>";
        }

        private Dictionary<string, object> ConvertBookingDataToDictionary(BookingEmailData data)
        {
            return new Dictionary<string, object>
            {
                { "GuestName", data.GuestName },
                { "GuestEmail", data.GuestEmail },
                { "BookingNumber", data.BookingNumber },
                { "RoomNumber", data.RoomNumber },
                { "RoomType", data.RoomType },
                { "CheckIn", data.CheckIn.ToString("yyyy-MM-dd") },
                { "CheckOut", data.CheckOut.ToString("yyyy-MM-dd") },
                { "TotalAmount", data.TotalAmount },
                { "HotelName", data.HotelName },
                { "HotelAddress", data.HotelAddress },
                { "HotelPhone", data.HotelPhone },
                { "SpecialRequests", data.SpecialRequests ?? "None" }
            };
        }

        private Dictionary<string, object> ConvertMaintenanceDataToDictionary(MaintenanceEmailData data)
        {
            return new Dictionary<string, object>
            {
                { "RoomNumber", data.RoomNumber },
                { "MaintenanceType", data.MaintenanceType },
                { "ScheduledDate", data.ScheduledDate.ToString("yyyy-MM-dd") },
                { "Description", data.Description },
                { "Notes", data.Notes ?? "None" },
                { "HotelName", data.HotelName }
            };
        }

        private Dictionary<string, object> ConvertPasswordResetDataToDictionary(PasswordResetEmailData data)
        {
            return new Dictionary<string, object>
            {
                { "UserName", data.UserName },
                { "ResetToken", data.ResetToken },
                { "ResetUrl", data.ResetUrl },
                { "ExpiryTime", data.ExpiryTime.ToString("yyyy-MM-dd HH:mm") }
            };
        }

        private (bool IsValid, string ErrorMessage) ValidateEmailAddresses(EmailDto emailDto)
        {
            if (string.IsNullOrWhiteSpace(emailDto.To))
            {
                return (false, "To email address is required");
            }

            if (!IsValidEmailAddress(emailDto.To))
            {
                return (false, $"Invalid To email address format: {emailDto.To}");
            }

            if (!string.IsNullOrWhiteSpace(emailDto.Cc) && !IsValidEmailAddress(emailDto.Cc))
            {
                return (false, $"Invalid CC email address format: {emailDto.Cc}");
            }

            if (!string.IsNullOrWhiteSpace(emailDto.Bcc) && !IsValidEmailAddress(emailDto.Bcc))
            {
                return (false, $"Invalid BCC email address format: {emailDto.Bcc}");
            }

            return (true, string.Empty);
        }

        private bool IsValidEmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Basic email format validation
                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(email))
                    return false;

                // Try to parse with MailKit to ensure it's valid
                var address = MailboxAddress.Parse(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GetDetailedErrorMessage(Exception ex)
        {
            return ex switch
            {
                ArgumentException argEx => $"Invalid email format: {argEx.Message}",
                InvalidOperationException opEx => $"Email operation failed: {opEx.Message}",
                System.Net.Sockets.SocketException sockEx => $"Network connection failed: {sockEx.Message}",
                AuthenticationException authEx => $"SMTP authentication failed: {authEx.Message}",
                SmtpCommandException smtpEx => $"SMTP command failed: {smtpEx.Message}",
                SmtpProtocolException protoEx => $"SMTP protocol error: {protoEx.Message}",
                _ => $"Unexpected error: {ex.Message}"
            };
        }
    }
}
