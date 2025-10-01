using HotelManagementApplication.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Services
{
    public interface IRoomService
    {
        Task<ICollection<RoomDto>> GetAllRoomsAsync();
        Task<RoomDto?> GetRoomByIdAsync(int id);
        Task<RoomDto?> GetRoomByNumberAsync(string roomNumber);
        Task<ICollection<RoomDto>> GetAvailableRoomsAsync();
        Task<ICollection<RoomDto>> GetRoomsByTypeAsync(string roomType);
        Task<ICollection<RoomDto>> GetRoomsByFloorAsync(int floor);
        Task<RoomDto> CreateRoomAsync(RoomCreateDto roomCreateDto);
        Task<RoomDto?> UpdateRoomAsync(RoomUpdateDto roomUpdateDto);
        Task<bool> DeleteRoomAsync(int id);
        Task<bool> UpdateRoomStatusAsync(int id, string status);
        Task<bool> MarkRoomAsCleanedAsync(int id);
        Task<bool> AddRoomImageAsync(int roomId, string imageUrl);
        Task<bool> RemoveRoomImageAsync(int roomId, string imageUrl);
        Task<bool> UpdateRoomImagesAsync(int roomId, List<string> imageUrls);
    }
}