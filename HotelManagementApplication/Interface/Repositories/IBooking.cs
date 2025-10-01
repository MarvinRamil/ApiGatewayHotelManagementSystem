using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Repositories
{
    public interface IBooking : IGeneric<Booking>
    {
        IQueryable<Booking> GetAll();
        Task<Booking?> GetByIdWithNavigationPropertiesAsync(int id);
        Task<Booking?> GetBookingByNumberAsync(string bookingNumber);
        Task<List<Booking>> GetBookingsByGuestIdAsync(int guestId);
        Task<List<Booking>> GetBookingsByRoomIdAsync(int roomId);
        Task<List<Booking>> GetBookingsByStatusAsync(BookingStatus status);
        Task<List<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Booking>> GetUpcomingBookingsAsync();
        Task<List<Booking>> GetActiveBookingsAsync();
        Task<List<Booking>> GetBookingsForTodayAsync();
        Task<List<Booking>> GetBookingsForDateAsync(DateTime date);
        Task<bool> HasOverlappingBookingsAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
    }
}
