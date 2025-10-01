using HootelManagementDomain.Entities;
using HotelManagementApplication.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Repositories
{
    public interface IRoom : IGeneric<Room>
    {
        IQueryable<Room> GetAllRooms();
        Task<RoomDto> createRoom(Room room);
        Task<Room?> GetRoomByNumberAsync(string roomNumber);
        Task<List<Room>> GetAvailableRoomsAsync();
        Task<List<Room>> GetRoomsByTypeAsync(string roomType);
        Task<List<Room>> GetRoomsByFloorAsync(int floor);
    }
}
