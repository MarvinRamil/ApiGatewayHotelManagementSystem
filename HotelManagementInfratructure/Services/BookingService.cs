using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementApplication.Interface.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementInfratructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingOtpService _bookingOtpService;

        public BookingService(IUnitOfWork unitOfWork, IBookingOtpService bookingOtpService)
        {
            _unitOfWork = unitOfWork;
            _bookingOtpService = bookingOtpService;
        }

        public async Task<ICollection<BookingResponseDto>> GetAllBookingsAsync()
        {
            return await _unitOfWork.Booking.GetAll()
                    .Select(b => new BookingResponseDto
                    {
                        Id = b.Id,
                        BookingNumber = b.BookingNumber,
                        Room = new RoomDto
                        {
                            Id = b.Room.Id,
                            Number = b.Room.Number,
                            Type = b.Room.Type,
                            Status = b.Room.Status,
                            Price = b.Room.Price,
                            Capacity = b.Room.Capacity,
                            Amenities = b.Room.Amenities,
                            Floor = b.Room.Floor,
                            LastCleaned = b.Room.LastCleaned,
                            CurrentGuest = b.Room.CurrentGuest,
                            Images = b.Room.Images,
                            Description = b.Room.Description,
                            CreatedAt = b.Room.CreatedBy,
                            UpdatedAt = b.Room.UpdatedAt
                        },
                        Guest = new GuestDto
                        {
                            Id = b.Guest.Id,
                            FirstName = b.Guest.FirstName,
                            LastName = b.Guest.LastName,
                            Email = b.Guest.Email,
                            Phone = b.Guest.Phone,
                            IdNumber = b.Guest.IdNumber,
                            Nationality = b.Guest.Nationality,
                            Address = b.Guest.Address,
                            Preferences = b.Guest.Preferences,
                            TotalStays = b.Guest.TotalStays,
                            TotalSpent = b.Guest.TotalSpent,
                            JoinDate = b.Guest.JoinDate,
                            IsActive = b.Guest.IsActive,
                            CreatedAt = b.Guest.CreatedBy,
                            UpdatedAt = b.Guest.UpdatedAt
                        },
                        CheckIn = b.CheckIn,
                        CheckOut = b.CheckOut,
                        Status = b.Status,
                        TotalAmount = b.TotalAmount,
                        CreatedAt = b.CreatedBy,
                    }).ToListAsync();
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Booking
                
                .GetAll()
                .Include(t => t.Room)
                .Include(t => t.Guest)
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();

            return booking != null ? MapToBookingResponseDto(booking) : null;
        }

        public async Task<ICollection<BookingResponseDto>> GetBookingsByGuestIdAsync(int guestId)
        {
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => b.GuestId == guestId)
                .Include(t => t.Guest)
                .Include(t => t.Room)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<ICollection<BookingResponseDto>> GetBookingsByRoomIdAsync(int roomId)
        {
            var bookings = await _unitOfWork.Booking.GetAll()
                .Include(t => t.Guest)
                .Include(t => t.Room)
                .Where(b => b.RoomId == roomId)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<ICollection<BookingResponseDto>> GetBookingsByStatusAsync(BookingStatus status)
        {
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => b.Status == status)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<ICollection<BookingResponseDto>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => b.CheckIn >= startDate && b.CheckOut <= endDate)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<BookingResponseDto> CreateBookingAsync(BookingCreateDto bookingCreateDto)
        {
            // Validate room availability
            var room = await _unitOfWork.Room.GetById(bookingCreateDto.RoomId);
            if (room == null)
            {
                throw new InvalidOperationException("Room not found.");
            }

            // Check for overlapping bookings
            var hasOverlappingBookings = await _unitOfWork.Booking.GetAll()
                .AnyAsync(b => b.RoomId == bookingCreateDto.RoomId &&
                              b.Status != BookingStatus.Cancelled &&
                              ((b.CheckIn <= bookingCreateDto.CheckIn && b.CheckOut > bookingCreateDto.CheckIn) ||
                               (b.CheckIn < bookingCreateDto.CheckOut && b.CheckOut >= bookingCreateDto.CheckOut) ||
                               (b.CheckIn >= bookingCreateDto.CheckIn && b.CheckOut <= bookingCreateDto.CheckOut)));

            if (hasOverlappingBookings)
            {
                throw new InvalidOperationException("Room is already booked for the selected dates.");
            }

            var hasmaintenace = await _unitOfWork.MaintenanceDate.GetMaintenanceDatesByDateRangeAsync(bookingCreateDto.CheckIn, bookingCreateDto.CheckOut.Date.AddHours(23).AddMinutes(59));

            if (hasmaintenace.Count() > 0)
            {
                throw new InvalidOperationException("Room is under maintenance for the selected dates.");
            }

            // Create or find guest
            var guest = await _unitOfWork.Guest.GetGuestByEmailAsync(bookingCreateDto.Email);
            if (guest == null)
            {
                guest = new Guest
                {
                    FirstName = bookingCreateDto.FirstName,
                    LastName = bookingCreateDto.LastName,
                    Email = bookingCreateDto.Email,
                    Phone = bookingCreateDto.Phone,
                    TotalStays = 0,
                    TotalSpent = 0,
                    JoinDate = DateTime.Now,
                    IsActive = true,
                    CreatedBy = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _unitOfWork.Guest.Add(guest);
                await _unitOfWork.SaveAsync();
            }

            // Calculate total amount
            var nights = (bookingCreateDto.CheckOut - bookingCreateDto.CheckIn).Days;
            var totalAmount = room.Price * nights;

            // Create booking
            var booking = new Booking
            {
                CheckIn = bookingCreateDto.CheckIn,
                CheckOut = bookingCreateDto.CheckOut,
                Guests = bookingCreateDto.Guests,
                Status = BookingStatus.Pending,
                TotalAmount = totalAmount,
                PaidAmount = 0,
                BookingNumber = GenerateBookingNumber(),
                PaymentMethod = "",
                SpecialRequests = bookingCreateDto.SpecialRequests,
                GuestId = guest.Id,
                RoomId = bookingCreateDto.RoomId,
                CreatedBy = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _unitOfWork.Booking.Add(booking);
            await _unitOfWork.SaveAsync();

            // Update room status
            room.Status = RoomStatus.Occupied;
            room.UpdatedAt = DateTime.Now;
            await _unitOfWork.Room.Update(room);
            await _unitOfWork.SaveAsync();

            // Generate and send OTP for booking confirmation
            try
            {
                await _bookingOtpService.GenerateOtpAsync(booking.Id, guest.Email);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the booking creation
                // The booking is created successfully, OTP can be generated later
                 await _unitOfWork.RollbackTransactionAsync();
                Console.WriteLine($"Warning: Failed to generate OTP for booking {booking.Id}: {ex.Message}");
            }

            return await GetBookingByIdAsync(booking.Id) ?? throw new InvalidOperationException("Failed to create booking.");
        }

        public async Task<BookingResponseDto?> UpdateBookingAsync(int id, BookingCreateDto bookingUpdateDto)
        {
            var booking = await _unitOfWork.Booking.GetById(id);
            if (booking == null)
            {
                return null;
            }

            // Update booking details
            booking.CheckIn = bookingUpdateDto.CheckIn;
            booking.CheckOut = bookingUpdateDto.CheckOut;
            booking.Guests = bookingUpdateDto.Guests;
            booking.SpecialRequests = bookingUpdateDto.SpecialRequests;
            booking.UpdatedAt = DateTime.UtcNow;

            // Recalculate total amount
            var room = await _unitOfWork.Room.GetById(booking.RoomId);
            if (room != null)
            {
                var nights = (bookingUpdateDto.CheckOut - bookingUpdateDto.CheckIn).Days;
                booking.TotalAmount = room.Price * nights;
            }

            await _unitOfWork.Booking.Update(booking);
            await _unitOfWork.SaveAsync();

            return await GetBookingByIdAsync(id);
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _unitOfWork.Booking.GetById(id);
            if (booking == null)
            {
                return false;
            }

            // Only allow deletion of cancelled or completed bookings
            if (booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.CheckedIn)
            {
                throw new InvalidOperationException("Cannot delete active bookings. Cancel the booking first.");
            }

            await _unitOfWork.Booking.Delete(booking);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateBookingStatusAsync(int id, BookingStatus status)
        {
            var booking = await _unitOfWork.Booking.GetById(id);
            if (booking == null)
            {
                return false;
            }

            booking.Status = status;
            booking.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Booking.Update(booking);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> CheckInBookingAsync(int id)
        {
            var booking = await _unitOfWork.Booking.GetById(id);
            if (booking == null || booking.Status != BookingStatus.Confirmed)
            {
                return false;
            }

            booking.Status = BookingStatus.CheckedIn;
            booking.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Booking.Update(booking);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> CheckOutBookingAsync(int id)
        {
            var booking = await _unitOfWork.Booking.GetById(id);
            if (booking == null || booking.Status != BookingStatus.CheckedIn)
            {
                return false;
            }

            booking.Status = BookingStatus.CheckedOut;
            booking.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Booking.Update(booking);
            await _unitOfWork.SaveAsync();

            // Update room status to available
            var room = await _unitOfWork.Room.GetById(booking.RoomId);
            if (room != null)
            {
                room.Status = RoomStatus.Available;
                room.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Room.Update(room);
                await _unitOfWork.SaveAsync();
            }

            return true;
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _unitOfWork.Booking.GetById(id);
            if (booking == null || booking.Status == BookingStatus.CheckedOut)
            {
                return false;
            }

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Booking.Update(booking);
            await _unitOfWork.SaveAsync();

            // Update room status to available
            var room = await _unitOfWork.Room.GetById(booking.RoomId);
            if (room != null)
            {
                room.Status = RoomStatus.Available;
                room.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Room.Update(room);
                await _unitOfWork.SaveAsync();
            }

            return true;
        }

        public async Task<ICollection<BookingResponseDto>> GetUpcomingBookingsAsync()
        {
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => b.CheckIn > DateTime.UtcNow && b.Status == BookingStatus.Confirmed)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<ICollection<BookingResponseDto>> GetActiveBookingsAsync()
        {
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => b.Status == BookingStatus.CheckedIn)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        private static BookingResponseDto MapToBookingResponseDto(Booking b)
        {
            return new BookingResponseDto
            {
                Id = b.Id,
                BookingNumber = b.BookingNumber,
                Room = new RoomDto
                {
                    Id = b.Room.Id,
                    Number = b.Room.Number,
                    Type = b.Room.Type,
                    Status = b.Room.Status,
                    Price = b.Room.Price,
                    Capacity = b.Room.Capacity,
                    Amenities = b.Room.Amenities,
                    Floor = b.Room.Floor,
                    LastCleaned = b.Room.LastCleaned,
                    CurrentGuest = b.Room.CurrentGuest,
                    Images = b.Room.Images,
                    Description = b.Room.Description,
                    CreatedAt = b.Room.CreatedBy,
                    UpdatedAt = b.Room.UpdatedAt
                },
                Guest = new GuestDto
                {
                    Id = b.Guest.Id,
                    FirstName = b.Guest.FirstName,
                    LastName = b.Guest.LastName,
                    Email = b.Guest.Email,
                    Phone = b.Guest.Phone,
                    IdNumber = b.Guest.IdNumber,
                    Nationality = b.Guest.Nationality,
                    Address = b.Guest.Address,
                    Preferences = b.Guest.Preferences,
                    TotalStays = b.Guest.TotalStays,
                    TotalSpent = b.Guest.TotalSpent,
                    JoinDate = b.Guest.JoinDate,
                    IsActive = b.Guest.IsActive,
                    CreatedAt = b.Guest.CreatedBy,
                    UpdatedAt = b.Guest.UpdatedAt
                },
                CheckIn = b.CheckIn,
                CheckOut = b.CheckOut,
                Status = b.Status,
                TotalAmount = b.TotalAmount,
                CreatedAt = b.CreatedBy,
            };
        }

        public async Task<ICollection<BookingResponseDto>> SearchBookingsAsync(BookingSearchDto searchDto)
        {
            var query = _unitOfWork.Booking.GetAll().AsQueryable();

            if (searchDto.StartDate.HasValue)
                query = query.Where(b => b.CheckIn >= searchDto.StartDate.Value);

            if (searchDto.EndDate.HasValue)
                query = query.Where(b => b.CheckOut <= searchDto.EndDate.Value);

            if (searchDto.GuestId.HasValue)
                query = query.Where(b => b.GuestId == searchDto.GuestId.Value);

            if (searchDto.RoomId.HasValue)
                query = query.Where(b => b.RoomId == searchDto.RoomId.Value);

            if (!string.IsNullOrEmpty(searchDto.BookingNumber))
                query = query.Where(b => b.BookingNumber.Contains(searchDto.BookingNumber));

            if (!string.IsNullOrEmpty(searchDto.GuestEmail))
                query = query.Where(b => b.Guest.Email.Contains(searchDto.GuestEmail));

            if (!string.IsNullOrEmpty(searchDto.GuestName))
                query = query.Where(b => (b.Guest.FirstName + " " + b.Guest.LastName).Contains(searchDto.GuestName));

            var bookings = await query.ToListAsync();
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<ICollection<BookingResponseDto>> GetBookingsByBookingNumberAsync(string bookingNumber)
        {
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => b.BookingNumber == bookingNumber)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<ICollection<BookingResponseDto>> GetBookingsByGuestEmailAsync(string email)
        {
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => b.Guest.Email == email)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<ICollection<BookingResponseDto>> GetBookingsByGuestNameAsync(string name)
        {
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => (b.Guest.FirstName + " " + b.Guest.LastName).Contains(name))
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<decimal> CalculateBookingTotalAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var room = await _unitOfWork.Room.GetById(roomId);
            if (room == null)
            {
                throw new InvalidOperationException("Room not found.");
            }

            var nights = (checkOut - checkIn).Days;
            return room.Price * nights;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            var room = await _unitOfWork.Room.GetById(roomId);
            if (room == null || room.Status != RoomStatus.Available)
            {
                return false;
            }

            var hasOverlappingBookings = await _unitOfWork.Booking.GetAll()
                .AnyAsync(b => b.RoomId == roomId &&
                              b.Status != BookingStatus.Cancelled &&
                              (excludeBookingId == null || b.Id != excludeBookingId) &&
                              ((b.CheckIn <= checkIn && b.CheckOut > checkIn) ||
                               (b.CheckIn < checkOut && b.CheckOut >= checkOut) ||
                               (b.CheckIn >= checkIn && b.CheckOut <= checkOut)));

            return !hasOverlappingBookings;
        }

        public async Task<ICollection<BookingResponseDto>> GetBookingsForTodayAsync()
        {
            var today = DateTime.UtcNow.Date;
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => b.CheckIn.Date == today || b.CheckOut.Date == today)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        public async Task<ICollection<BookingResponseDto>> GetBookingsForDateAsync(DateTime date)
        {
            var targetDate = date.Date;
            var bookings = await _unitOfWork.Booking.GetAll()
                .Where(b => b.CheckIn.Date <= targetDate && b.CheckOut.Date >= targetDate)
                .ToListAsync();
            
            return bookings.Select(MapToBookingResponseDto).ToList();
        }

        private static string GenerateBookingNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"BK{timestamp}{random}";
        }
    }
}
