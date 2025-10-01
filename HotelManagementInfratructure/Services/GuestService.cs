using HootelManagementDomain.Entities;
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
    public class GuestService : IGuestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GuestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ICollection<GuestDto>> GetAllGuestsAsync()
        {
            var guests = await _unitOfWork.Guest.GetAllAsync();
            return guests.Select(MapToDto).ToList();
        }

        public async Task<GuestDto?> GetGuestByIdAsync(int id)
        {
            var guest = await _unitOfWork.Guest.GetById(id);
            return guest != null ? MapToDto(guest) : null;
        }

        public async Task<GuestDto?> GetGuestByEmailAsync(string email)
        {
            var guest = await _unitOfWork.Guest.GetGuestByEmailAsync(email);
            return guest != null ? MapToDto(guest) : null;
        }

        public async Task<ICollection<GuestDto>> GetActiveGuestsAsync()
        {
            var guests = await _unitOfWork.Guest.GetActiveGuestsAsync();
            return guests.Select(MapToDto).ToList();
        }

        public async Task<ICollection<GuestDto>> GetGuestsByNationalityAsync(string nationality)
        {
            var guests = await _unitOfWork.Guest.GetGuestsByNationalityAsync(nationality);
            return guests.Select(MapToDto).ToList();
        }

        public async Task<GuestDto> CreateGuestAsync(GuestCreateDto guestCreateDto)
        {
            // Check if guest with email already exists
            var existingGuest = await _unitOfWork.Guest.GetGuestByEmailAsync(guestCreateDto.Email);
            if (existingGuest != null)
            {
                throw new InvalidOperationException($"Guest with email {guestCreateDto.Email} already exists.");
            }

            var guest = new Guest
            {
                FirstName = guestCreateDto.FirstName,
                LastName = guestCreateDto.LastName,
                Email = guestCreateDto.Email,
                Phone = guestCreateDto.Phone,
                IdNumber = guestCreateDto.IdNumber,
                Nationality = guestCreateDto.Nationality,
                Address = guestCreateDto.Address,
                Preferences = guestCreateDto.Preferences,
                TotalStays = 0,
                TotalSpent = 0,
                JoinDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Guest.Add(guest);
            await _unitOfWork.SaveAsync();

            return MapToDto(guest);
        }

        public async Task<GuestDto?> UpdateGuestAsync(GuestUpdateDto guestUpdateDto)
        {
            var guest = await _unitOfWork.Guest.GetById(guestUpdateDto.Id);
            if (guest == null)
            {
                return null;
            }

            // Check if email already exists (excluding current guest)
            var existingGuest = await _unitOfWork.Guest.GetGuestByEmailAsync(guestUpdateDto.Email);
            if (existingGuest != null && existingGuest.Id != guestUpdateDto.Id)
            {
                throw new InvalidOperationException($"Guest with email {guestUpdateDto.Email} already exists.");
            }

            guest.FirstName = guestUpdateDto.FirstName;
            guest.LastName = guestUpdateDto.LastName;
            guest.Email = guestUpdateDto.Email;
            guest.Phone = guestUpdateDto.Phone;
            guest.IdNumber = guestUpdateDto.IdNumber;
            guest.Nationality = guestUpdateDto.Nationality;
            guest.Address = guestUpdateDto.Address;
            guest.Preferences = guestUpdateDto.Preferences;
            guest.IsActive = guestUpdateDto.IsActive;
            guest.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Guest.Update(guest);
            await _unitOfWork.SaveAsync();

            return MapToDto(guest);
        }

        public async Task<bool> DeleteGuestAsync(int id)
        {
            var guest = await _unitOfWork.Guest.GetById(id);
            if (guest == null)
            {
                return false;
            }

            // Check if guest has active bookings
            var hasActiveBookings = await _unitOfWork.Booking.GetAll()
                .AnyAsync(b => b.GuestId == id && 
                              (b.Status == HootelManagementDomain.enums.BookingStatus.Confirmed || 
                               b.Status == HootelManagementDomain.enums.BookingStatus.CheckedIn));

            if (hasActiveBookings)
            {
                throw new InvalidOperationException("Cannot delete guest with active bookings.");
            }

            await _unitOfWork.Guest.Delete(guest);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeactivateGuestAsync(int id)
        {
            var guest = await _unitOfWork.Guest.GetById(id);
            if (guest == null)
            {
                return false;
            }

            guest.IsActive = false;
            guest.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Guest.Update(guest);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> ActivateGuestAsync(int id)
        {
            var guest = await _unitOfWork.Guest.GetById(id);
            if (guest == null)
            {
                return false;
            }

            guest.IsActive = true;
            guest.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Guest.Update(guest);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<ICollection<GuestDto>> GetTopSpendingGuestsAsync(int count = 10)
        {
            var guests = await _unitOfWork.Guest.GetTopSpendingGuestsAsync(count);
            return guests.Select(MapToDto).ToList();
        }

        public async Task<ICollection<GuestDto>> GetFrequentGuestsAsync(int minStays = 3)
        {
            var guests = await _unitOfWork.Guest.GetFrequentGuestsAsync(minStays);
            return guests.Select(MapToDto).ToList();
        }

        private static GuestDto MapToDto(Guest guest)
        {
            return new GuestDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                Email = guest.Email,
                Phone = guest.Phone,
                IdNumber = guest.IdNumber,
                Nationality = guest.Nationality,
                Address = guest.Address,
                Preferences = guest.Preferences,
                TotalStays = guest.TotalStays,
                TotalSpent = guest.TotalSpent,
                JoinDate = guest.JoinDate,
                IsActive = guest.IsActive,
                CreatedAt = guest.CreatedBy,
                UpdatedAt = guest.UpdatedAt
            };
        }
    }
}
