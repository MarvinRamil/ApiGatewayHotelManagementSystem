using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementApplication.Interface.Services;
using HootelManagementDomain.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingServiceApi.Extensions;

namespace BookManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync();
                return this.CreateResponse(200, "Bookings retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving bookings: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        //[Authorize]
        public async Task<IActionResult> GetBookingById(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    return this.CreateResponse(404, "Booking not found");
                }
                return this.CreateResponse(200, "Booking retrieved successfully", booking);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving booking: {ex.Message}");
            }
        }

        [HttpGet("guest/{guestId}")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsByGuestId(int guestId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByGuestIdAsync(guestId);
                return this.CreateResponse(200, "Guest bookings retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving guest bookings: {ex.Message}");
            }
        }

        [HttpGet("room/{roomId}")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsByRoomId(int roomId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByRoomIdAsync(roomId);
                return this.CreateResponse(200, "Room bookings retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving room bookings: {ex.Message}");
            }
        }

        [HttpGet("status/{status}")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsByStatus(BookingStatus status)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByStatusAsync(status);
                return this.CreateResponse(200, "Bookings by status retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving bookings by status: {ex.Message}");
            }
        }

        [HttpGet("date-range")]
        [Authorize]
        public async Task<IActionResult> GetBookingsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByDateRangeAsync(startDate, endDate);
                return this.CreateResponse(200, "Bookings by date range retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving bookings by date range: {ex.Message}");
            }
        }

        [HttpGet("upcoming")]
        [Authorize]
        public async Task<IActionResult> GetUpcomingBookings()
        {
            try
            {
                var bookings = await _bookingService.GetUpcomingBookingsAsync();
                return this.CreateResponse(200, "Upcoming bookings retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving upcoming bookings: {ex.Message}");
            }
        }

        [HttpGet("active")]
        [Authorize]
        public async Task<IActionResult> GetActiveBookings()
        {
            try
            {
                var bookings = await _bookingService.GetActiveBookingsAsync();
                return this.CreateResponse(200, "Active bookings retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving active bookings: {ex.Message}");
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid booking data", ModelState);
                }

                var booking = await _bookingService.CreateBookingAsync(bookingCreateDto);
                return this.CreateResponse(201, "Booking created successfully", booking);
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error creating booking: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingCreateDto bookingUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid booking data", ModelState);
                }

                var booking = await _bookingService.UpdateBookingAsync(id, bookingUpdateDto);
                if (booking == null)
                {
                    return this.CreateResponse(404, "Booking not found");
                }
                return this.CreateResponse(200, "Booking updated successfully", booking);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error updating booking: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var result = await _bookingService.DeleteBookingAsync(id);
                if (!result)
                {
                    return this.CreateResponse(404, "Booking not found");
                }
                return this.CreateResponse(200, "Booking deleted successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error deleting booking: {ex.Message}");
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] BookingStatus status)
        {
            try
            {
                var result = await _bookingService.UpdateBookingStatusAsync(id, status);
                if (!result)
                {
                    return this.CreateResponse(404, "Booking not found");
                }
                return this.CreateResponse(200, "Booking status updated successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error updating booking status: {ex.Message}");
            }
        }

        [HttpPatch("{id}/check-in")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> CheckInBooking(int id)
        {
            try
            {
                var result = await _bookingService.CheckInBookingAsync(id);
                if (!result)
                {
                    return this.CreateResponse(400, "Booking not found or cannot be checked in");
                }
                return this.CreateResponse(200, "Booking checked in successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error checking in booking: {ex.Message}");
            }
        }

        [HttpPatch("{id}/check-out")]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> CheckOutBooking(int id)
        {
            try
            {
                var result = await _bookingService.CheckOutBookingAsync(id);
                if (!result)
                {
                    return this.CreateResponse(400, "Booking not found or cannot be checked out");
                }
                return this.CreateResponse(200, "Booking checked out successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error checking out booking: {ex.Message}");
            }
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(id);
                if (!result)
                {
                    return this.CreateResponse(400, "Booking not found or cannot be cancelled");
                }
                return this.CreateResponse(200, "Booking cancelled successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error cancelling booking: {ex.Message}");
            }
        }

        [HttpPost("search")]
        [Authorize]
        public async Task<IActionResult> SearchBookings([FromBody] BookingSearchDto searchDto)
        {
            try
            {
                var bookings = await _bookingService.SearchBookingsAsync(searchDto);
                return this.CreateResponse(200, "Bookings search completed successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error searching bookings: {ex.Message}");
            }
        }

        [HttpGet("booking-number/{bookingNumber}")]
        [Authorize]
        public async Task<IActionResult> GetBookingsByBookingNumber(string bookingNumber)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByBookingNumberAsync(bookingNumber);
                return this.CreateResponse(200, "Bookings by booking number retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving bookings by booking number: {ex.Message}");
            }
        }

        [HttpGet("guest-email/{email}")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsByGuestEmail(string email)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByGuestEmailAsync(email);
                return this.CreateResponse(200, "Bookings by guest email retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving bookings by guest email: {ex.Message}");
            }
        }

        [HttpGet("guest-name/{name}")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsByGuestName(string name)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByGuestNameAsync(name);
                return this.CreateResponse(200, "Bookings by guest name retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving bookings by guest name: {ex.Message}");
            }
        }

        [HttpGet("calculate-total")]
        //[Authorize]
        public async Task<IActionResult> CalculateBookingTotal([FromQuery] int roomId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            try
            {
                var total = await _bookingService.CalculateBookingTotalAsync(roomId, checkIn, checkOut);
                return this.CreateResponse(200, "Booking total calculated successfully", new { Total = total });
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error calculating booking total: {ex.Message}");
            }
        }

        [HttpGet("check-availability")]
        //[Authorize]
        public async Task<IActionResult> CheckRoomAvailability([FromQuery] int roomId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, [FromQuery] int? excludeBookingId = null)
        {
            try
            {
                var isAvailable = await _bookingService.IsRoomAvailableAsync(roomId, checkIn, checkOut, excludeBookingId);
                return this.CreateResponse(200, "Room availability checked successfully", new { IsAvailable = isAvailable });
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error checking room availability: {ex.Message}");
            }
        }

        [HttpGet("today")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsForToday()
        {
            try
            {
                var bookings = await _bookingService.GetBookingsForTodayAsync();
                return this.CreateResponse(200, "Today's bookings retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving today's bookings: {ex.Message}");
            }
        }

        [HttpGet("date/{date}")]
        //[Authorize]
        public async Task<IActionResult> GetBookingsForDate(DateTime date)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsForDateAsync(date);
                return this.CreateResponse(200, "Bookings for date retrieved successfully", bookings);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving bookings for date: {ex.Message}");
            }
        }
    }
}