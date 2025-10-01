using HotelManagementApplication.Dto;
using HootelManagementDomain.Entities;

namespace HotelManagementApplication.Interface.Repositories
{
    public interface IBookingOtp : IGeneric<BookingOtp>
    {
        Task<BookingOtp?> GetByBookingIdAsync(int bookingId);
        Task<BookingOtp?> GetByOtpCodeAsync(string otpCode);
        Task<IEnumerable<BookingOtp>> GetExpiredOtpsAsync();
        Task<bool> IsOtpValidAsync(int bookingId, string otpCode);
        Task<bool> HasActiveOtpAsync(int bookingId);
        Task<int> DeleteExpiredOtpsAsync();
    }
}
