using HootelManagementDomain.Entities;
using HotelManagementApplication.Dto;
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
    public class GuestRepository : GenericRepository<Guest>, IGuest
    {
        public GuestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Guest> GetAllGuests()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }

        public async Task<GuestDto> createGuest(Guest guest)
        {
            await Add(guest);
            await _context.SaveChangesAsync();
            
            return MapToDto(guest);
        }

        public async Task<Guest?> GetGuestByEmailAsync(string email)
        {
            var aaaa = await _dbSet
                .FirstOrDefaultAsync(g => g.Email == email);
            return await _dbSet
                .FirstOrDefaultAsync(g => g.Email == email);
        }

        public async Task<List<Guest>> GetActiveGuestsAsync()
        {
            return await _dbSet
                .Where(g => g.IsActive)
                .ToListAsync();
        }

        public async Task<List<Guest>> GetGuestsByNationalityAsync(string nationality)
        {
            return await _dbSet
                .Where(g => g.Nationality == nationality)
                .ToListAsync();
        }

        public async Task<List<Guest>> GetTopSpendingGuestsAsync(int count = 10)
        {
            return await _dbSet
                .OrderByDescending(g => g.TotalSpent)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Guest>> GetFrequentGuestsAsync(int minStays = 3)
        {
            return await _dbSet
                .Where(g => g.TotalStays >= minStays)
                .OrderByDescending(g => g.TotalStays)
                .ToListAsync();
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
