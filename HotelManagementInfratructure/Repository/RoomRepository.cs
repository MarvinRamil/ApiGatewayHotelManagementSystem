using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
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
    public class RoomRepository : GenericRepository<Room>, IRoom
    {
        public RoomRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Room> GetAllRooms()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }

        public async Task<RoomDto> createRoom(Room room)
        {
            await Add(room);
            await _context.SaveChangesAsync();
            
            return MapToDto(room);
        }

        public async Task<Room?> GetRoomByNumberAsync(string roomNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Number == roomNumber);
        }

        public async Task<List<Room>> GetAvailableRoomsAsync()
        {
            return await _dbSet
                .Where(r => r.Status == RoomStatus.Available)
                .ToListAsync();
        }

        public async Task<List<Room>> GetRoomsByTypeAsync(string roomType)
        {
            return await _dbSet
                .Where(r => r.Type == roomType)
                .ToListAsync();
        }

        public async Task<List<Room>> GetRoomsByFloorAsync(int floor)
        {
            return await _dbSet
                .Where(r => r.Floor == floor)
                .ToListAsync();
        }

        private static RoomDto MapToDto(Room room)
        {
            return new RoomDto
            {
                Id = room.Id,
                Number = room.Number,
                Type = room.Type,
                Status = room.Status,
                Price = room.Price,
                Capacity = room.Capacity,
                Amenities = room.Amenities,
                Floor = room.Floor,
                LastCleaned = room.LastCleaned,
                CurrentGuest = room.CurrentGuest,
                Images = room.Images,
                Description = room.Description,
                CreatedAt = room.CreatedBy,
                UpdatedAt = room.UpdatedAt
            };
        }
    }
}
