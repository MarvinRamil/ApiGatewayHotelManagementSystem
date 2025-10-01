using HotelManagementApplication.Dto;

namespace HotelManagementApplication.Interface.Services
{
    public interface IBookingOtpService
    {
        Task<ApiResponse<BookingOtpDto>> GenerateOtpAsync(int bookingId, string email);
        Task<ApiResponse<BookingOtpResponseDto>> VerifyOtpAsync(int bookingId, string otpCode);
        Task<ApiResponse<BookingOtpDto>> GetOtpByBookingIdAsync(int bookingId);
        Task<ApiResponse<bool>> ResendOtpAsync(int bookingId);
        Task<ApiResponse<bool>> ExpireOtpAsync(int bookingId);
        Task<ApiResponse<IEnumerable<BookingOtpDto>>> GetExpiredOtpsAsync();
        Task<ApiResponse<bool>> CleanupExpiredOtpsAsync();
    }
}
