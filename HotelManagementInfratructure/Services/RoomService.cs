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
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ICollection<RoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _unitOfWork.Room.GetAllAsync();
            return rooms.Select(MapToDto).ToList();
        }

        public async Task<RoomDto?> GetRoomByIdAsync(int id)
        {
            var room = await _unitOfWork.Room.GetById(id);
            return room != null ? MapToDto(room) : null;
        }

        public async Task<RoomDto?> GetRoomByNumberAsync(string roomNumber)
        {
            var room = await _unitOfWork.Room.GetRoomByNumberAsync(roomNumber);
            return room != null ? MapToDto(room) : null;
        }

        public async Task<ICollection<RoomDto>> GetAvailableRoomsAsync()
        {
            var rooms = await _unitOfWork.Room.GetAvailableRoomsAsync();
            return rooms.Select(MapToDto).ToList();
        }

        public async Task<ICollection<RoomDto>> GetRoomsByTypeAsync(string roomType)
        {
            var rooms = await _unitOfWork.Room.GetRoomsByTypeAsync(roomType);
            return rooms.Select(MapToDto).ToList();
        }

        public async Task<ICollection<RoomDto>> GetRoomsByFloorAsync(int floor)
        {
            var rooms = await _unitOfWork.Room.GetRoomsByFloorAsync(floor);
            return rooms.Select(MapToDto).ToList();
        }

        public async Task<RoomDto> CreateRoomAsync(RoomCreateDto roomCreateDto)
        {
            // Check if room number already exists
            var existingRoom = await _unitOfWork.Room.GetRoomByNumberAsync(roomCreateDto.Number);
            if (existingRoom != null)
            {
                throw new InvalidOperationException($"Room with number {roomCreateDto.Number} already exists.");
            }

            var room = new Room
            {
                Number = roomCreateDto.Number,
                Type = roomCreateDto.Type,
                Status = RoomStatus.Available,
                Price = roomCreateDto.Price,
                Capacity = roomCreateDto.Capacity,
                Amenities = roomCreateDto.Amenities,
                Floor = roomCreateDto.Floor,
                Description = roomCreateDto.Description,
                Images = [],
                CreatedBy = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Room.Add(room);
            await _unitOfWork.SaveAsync();

            return MapToDto(room);
        }

        public async Task<RoomDto?> UpdateRoomAsync(RoomUpdateDto roomUpdateDto)
        {
            var room = await _unitOfWork.Room.GetById(roomUpdateDto.Id);
            if (room == null)
            {
                return null;
            }

            // Check if room number already exists (excluding current room)
            var existingRoom = await _unitOfWork.Room.GetRoomByNumberAsync(roomUpdateDto.Number);
            if (existingRoom != null && existingRoom.Id != roomUpdateDto.Id)
            {
                throw new InvalidOperationException($"Room with number {roomUpdateDto.Number} already exists.");
            }

            room.Number = roomUpdateDto.Number;
            room.Type = roomUpdateDto.Type;
            room.Price = roomUpdateDto.Price;
            room.Capacity = roomUpdateDto.Capacity;
            room.Amenities = roomUpdateDto.Amenities;
            room.Floor = roomUpdateDto.Floor;
            room.Description = roomUpdateDto.Description;
            room.Images = roomUpdateDto.Images;
            room.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Room.Update(room);
            await _unitOfWork.SaveAsync();

            return MapToDto(room);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _unitOfWork.Room.GetById(id);
            if (room == null)
            {
                return false;
            }

            // Check if room has active bookings
            var hasActiveBookings = await _unitOfWork.Booking.GetAll()
                .AnyAsync(b => b.RoomId == id && 
                              (b.Status == BookingStatus.Confirmed || 
                               b.Status == BookingStatus.CheckedIn));

            if (hasActiveBookings)
            {
                throw new InvalidOperationException("Cannot delete room with active bookings.");
            }

            await _unitOfWork.Room.Delete(room);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateRoomStatusAsync(int id, string status)
        {
            var room = await _unitOfWork.Room.GetById(id);
            if (room == null)
            {
                return false;
            }

            if (Enum.TryParse<RoomStatus>(status, true, out var roomStatus))
            {
                room.Status = roomStatus;
                room.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Room.Update(room);
                await _unitOfWork.SaveAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> MarkRoomAsCleanedAsync(int id)
        {
            var room = await _unitOfWork.Room.GetById(id);
            if (room == null)
            {
                return false;
            }

            room.LastCleaned = DateTime.UtcNow;
            room.Status = RoomStatus.Available;
            room.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Room.Update(room);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> AddRoomImageAsync(int roomId, string imageUrl)
        {
            var room = await _unitOfWork.Room.GetById(roomId);
            if (room == null)
            {
                return false;
            }

            if (!room.Images.Contains(imageUrl))
            {
                room.Images.Add(imageUrl);
                room.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Room.Update(room);
                await _unitOfWork.SaveAsync();
            }
            return true;
        }

        public async Task<bool> RemoveRoomImageAsync(int roomId, string imageUrl)
        {
            var room = await _unitOfWork.Room.GetById(roomId);
            if (room == null)
            {
                return false;
            }

            if (room.Images.Contains(imageUrl))
            {
                room.Images.Remove(imageUrl);
                room.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Room.Update(room);
                await _unitOfWork.SaveAsync();
            }
            return true;
        }

        public async Task<bool> UpdateRoomImagesAsync(int roomId, List<string> imageUrls)
        {
            var room = await _unitOfWork.Room.GetById(roomId);
            if (room == null)
            {
                return false;
            }

            room.Images = imageUrls;
            room.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Room.Update(room);
            await _unitOfWork.SaveAsync();
            return true;
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
