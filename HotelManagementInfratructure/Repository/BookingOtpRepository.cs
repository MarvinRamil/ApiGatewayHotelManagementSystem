using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementInfratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;

namespace HotelManagementInfratructure.Repository
{
    public class BookingOtpRepository : GenericRepository<BookingOtp>, IBookingOtp
    {
        public BookingOtpRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<BookingOtp?> GetByBookingIdAsync(int bookingId)
        {
            return await _context.BookingOtps
                .Include(b => b.Booking)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<BookingOtp?> GetByOtpCodeAsync(string otpCode)
        {
            return await _context.BookingOtps
                .Include(b => b.Booking)
                .FirstOrDefaultAsync(b => b.OtpCode == otpCode);
        }

        public async Task<IEnumerable<BookingOtp>> GetExpiredOtpsAsync()
        {
            return await _context.BookingOtps
                .Where(b => b.ExpiresAt < DateTime.UtcNow && b.Status == OtpStatus.Pending)
                .ToListAsync();
        }

        public async Task<bool> IsOtpValidAsync(int bookingId, string otpCode)
        {
            var otp = await _context.BookingOtps
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && 
                                         b.OtpCode == otpCode && 
                                         b.ExpiresAt > DateTime.UtcNow && 
                                         !b.IsUsed && 
                                         b.Status == OtpStatus.Pending);

            return otp != null;
        }

        public async Task<bool> HasActiveOtpAsync(int bookingId)
        {
            return await _context.BookingOtps
                .AnyAsync(b => b.BookingId == bookingId && 
                              b.ExpiresAt > DateTime.UtcNow && 
                              !b.IsUsed && 
                              b.Status == OtpStatus.Pending);
        }

        public async Task<int> DeleteExpiredOtpsAsync()
        {
            var expiredOtps = await _context.BookingOtps
                .Where(b => b.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            _context.BookingOtps.RemoveRange(expiredOtps);
            return await _context.SaveChangesAsync();
        }

        private BookingOtpDto MapToDto(BookingOtp otp)
        {
            return new BookingOtpDto
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
            };
        }
    }
}
