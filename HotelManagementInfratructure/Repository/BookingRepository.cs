using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using HotelManagementApplication.Interface;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementInfratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementInfratructure.Repository
{
    public class BookingRepository : GenericRepository<Booking>, IBooking
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Booking?> GetByIdWithNavigationPropertiesAsync(int id)
        {
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public IQueryable<Booking> GetAll()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }

        public async Task<Booking?> GetBookingByNumberAsync(string bookingNumber)
        {
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
        }

        public async Task<List<Booking>> GetBookingsByGuestIdAsync(int guestId)
        {
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .Where(b => b.GuestId == guestId)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId)
        {
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .Where(b => b.RoomId == roomId)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .Where(b => b.Status == status)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .Where(b => b.CheckIn >= startDate && b.CheckOut <= endDate)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetUpcomingBookingsAsync()
        {
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .Where(b => b.CheckIn > DateTime.UtcNow && b.Status == BookingStatus.Confirmed)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetActiveBookingsAsync()
        {
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .Where(b => b.Status == BookingStatus.CheckedIn)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsForTodayAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .Where(b => b.CheckIn.Date == today || b.CheckOut.Date == today)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingsForDateAsync(DateTime date)
        {
            var targetDate = date.Date;
            return await _dbSet
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .Where(b => b.CheckIn.Date <= targetDate && b.CheckOut.Date >= targetDate)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingBookingsAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            return await _dbSet
                .AnyAsync(b => b.RoomId == roomId &&
                              b.Status != BookingStatus.Cancelled &&
                              (excludeBookingId == null || b.Id != excludeBookingId) &&
                              ((b.CheckIn <= checkIn && b.CheckOut > checkIn) ||
                               (b.CheckIn < checkOut && b.CheckOut >= checkOut) ||
                               (b.CheckIn >= checkIn && b.CheckOut <= checkOut)));
        }
    }
}
