using HotelManagementApplication.Dto;
using HootelManagementDomain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Services
{
    public interface IBookingService
    {
        Task<ICollection<BookingResponseDto>> GetAllBookingsAsync();
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<ICollection<BookingResponseDto>> GetBookingsByGuestIdAsync(int guestId);
        Task<ICollection<BookingResponseDto>> GetBookingsByRoomIdAsync(int roomId);
        Task<ICollection<BookingResponseDto>> GetBookingsByStatusAsync(BookingStatus status);
        Task<ICollection<BookingResponseDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<BookingResponseDto> CreateBookingAsync(BookingCreateDto bookingCreateDto);
        Task<BookingResponseDto?> UpdateBookingAsync(int id, BookingCreateDto bookingUpdateDto);
        Task<bool> DeleteBookingAsync(int id);
        Task<bool> UpdateBookingStatusAsync(int id, BookingStatus status);
        Task<bool> CheckInBookingAsync(int id);
        Task<bool> CheckOutBookingAsync(int id);
        Task<bool> CancelBookingAsync(int id);
        Task<ICollection<BookingResponseDto>> GetUpcomingBookingsAsync();
        Task<ICollection<BookingResponseDto>> GetActiveBookingsAsync();
        Task<ICollection<BookingResponseDto>> SearchBookingsAsync(BookingSearchDto searchDto);
        Task<ICollection<BookingResponseDto>> GetBookingsByBookingNumberAsync(string bookingNumber);
        Task<ICollection<BookingResponseDto>> GetBookingsByGuestEmailAsync(string email);
        Task<ICollection<BookingResponseDto>> GetBookingsByGuestNameAsync(string name);
        Task<decimal> CalculateBookingTotalAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
        Task<ICollection<BookingResponseDto>> GetBookingsForTodayAsync();
        Task<ICollection<BookingResponseDto>> GetBookingsForDateAsync(DateTime date);
    }
}
