using HotelManagementApplication.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Services
{
    public interface IGuestService
    {
        Task<ICollection<GuestDto>> GetAllGuestsAsync();
        Task<GuestDto?> GetGuestByIdAsync(int id);
        Task<GuestDto?> GetGuestByEmailAsync(string email);
        Task<ICollection<GuestDto>> GetActiveGuestsAsync();
        Task<ICollection<GuestDto>> GetGuestsByNationalityAsync(string nationality);
        Task<GuestDto> CreateGuestAsync(GuestCreateDto guestCreateDto);
        Task<GuestDto?> UpdateGuestAsync(GuestUpdateDto guestUpdateDto);
        Task<bool> DeleteGuestAsync(int id);
        Task<bool> DeactivateGuestAsync(int id);
        Task<bool> ActivateGuestAsync(int id);
        Task<ICollection<GuestDto>> GetTopSpendingGuestsAsync(int count = 10);
        Task<ICollection<GuestDto>> GetFrequentGuestsAsync(int minStays = 3);
    }
}
