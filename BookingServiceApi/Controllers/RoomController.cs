using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingServiceApi.Extensions;

namespace BookManagementSystem.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync();
                return this.CreateResponse(200, "Rooms retrieved successfully", rooms);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving rooms: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(id);
                if (room == null)
                {
                    return this.CreateResponse(404, "Room not found");
                }
                return this.CreateResponse(200, "Room retrieved successfully", room);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving room: {ex.Message}");
            }
        }

        [HttpGet("number/{roomNumber}")]
        public async Task<IActionResult> GetRoomByNumber(string roomNumber)
        {
            try
            {
                var room = await _roomService.GetRoomByNumberAsync(roomNumber);
                if (room == null)
                {
                    return this.CreateResponse(404, "Room not found");
                }
                return this.CreateResponse(200, "Room retrieved successfully", room);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving room: {ex.Message}");
            }
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms()
        {
            try
            {
                var rooms = await _roomService.GetAvailableRoomsAsync();
                return this.CreateResponse(200, "Available rooms retrieved successfully", rooms);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving available rooms: {ex.Message}");
            }
        }

        [HttpGet("type/{roomType}")]
        public async Task<IActionResult> GetRoomsByType(string roomType)
        {
            try
            {
                var rooms = await _roomService.GetRoomsByTypeAsync(roomType);
                return this.CreateResponse(200, $"Rooms of type {roomType} retrieved successfully", rooms);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving rooms by type: {ex.Message}");
            }
        }

        [HttpGet("floor/{floor}")]
        public async Task<IActionResult> GetRoomsByFloor(int floor)
        {
            try
            {
                var rooms = await _roomService.GetRoomsByFloorAsync(floor);
                return this.CreateResponse(200, $"Rooms on floor {floor} retrieved successfully", rooms);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving rooms by floor: {ex.Message}");
            }
        }

        [HttpPost]
       // [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomCreateDto roomCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid room data", ModelState);
                }

                var room = await _roomService.CreateRoomAsync(roomCreateDto);
                return this.CreateResponse(201, "Room created successfully", room);
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error creating room: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
       // [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomUpdateDto roomUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid room data", ModelState);
                }

                roomUpdateDto.Id = id;
                var room = await _roomService.UpdateRoomAsync(roomUpdateDto);
                if (room == null)
                {
                    return this.CreateResponse(404, "Room not found");
                }
                return this.CreateResponse(200, "Room updated successfully", room);
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error updating room: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var result = await _roomService.DeleteRoomAsync(id);
                if (!result)
                {
                    return this.CreateResponse(404, "Room not found");
                }
                return this.CreateResponse(200, "Room deleted successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error deleting room: {ex.Message}");
            }
        }

        [HttpPatch("{id}/status")]
        //[Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> UpdateRoomStatus(int id, [FromBody] string status)
        {
            try
            {
                var result = await _roomService.UpdateRoomStatusAsync(id, status);
                if (!result)
                {
                    return this.CreateResponse(404, "Room not found or invalid status");
                }
                return this.CreateResponse(200, "Room status updated successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error updating room status: {ex.Message}");
            }
        }

        [HttpPatch("{id}/clean")]
       // [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> MarkRoomAsCleaned(int id)
        {
            try
            {
                var result = await _roomService.MarkRoomAsCleanedAsync(id);
                if (!result)
                {
                    return this.CreateResponse(404, "Room not found");
                }
                return this.CreateResponse(200, "Room marked as cleaned successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error marking room as cleaned: {ex.Message}");
            }
        }
    }
}
