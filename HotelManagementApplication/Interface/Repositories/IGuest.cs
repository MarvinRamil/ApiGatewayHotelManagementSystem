using HootelManagementDomain.Entities;
using HotelManagementApplication.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Repositories
{
    public interface IGuest : IGeneric<Guest>
    {
        IQueryable<Guest> GetAllGuests();
        Task<GuestDto> createGuest(Guest guest);
        Task<Guest?> GetGuestByEmailAsync(string email);
        Task<List<Guest>> GetActiveGuestsAsync();
        Task<List<Guest>> GetGuestsByNationalityAsync(string nationality);
        Task<List<Guest>> GetTopSpendingGuestsAsync(int count = 10);
        Task<List<Guest>> GetFrequentGuestsAsync(int minStays = 3);
    }
}