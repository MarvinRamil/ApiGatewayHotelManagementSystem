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
    public class GuestController : ControllerBase
    {
        private readonly IGuestService _guestService;

        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGuests()
        {
            try
            {
                var guests = await _guestService.GetAllGuestsAsync();
                return this.CreateResponse(200, "Guests retrieved successfully", guests);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving guests: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGuestById(int id)
        {
            try
            {
                var guest = await _guestService.GetGuestByIdAsync(id);
                if (guest == null)
                {
                    return this.CreateResponse(404, "Guest not found");
                }
                return this.CreateResponse(200, "Guest retrieved successfully", guest);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving guest: {ex.Message}");
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetGuestByEmail(string email)
        {
            try
            {
                var guest = await _guestService.GetGuestByEmailAsync(email);
                if (guest == null)
                {
                    return this.CreateResponse(404, "Guest not found");
                }
                return this.CreateResponse(200, "Guest retrieved successfully", guest);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving guest: {ex.Message}");
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveGuests()
        {
            try
            {
                var guests = await _guestService.GetActiveGuestsAsync();
                return this.CreateResponse(200, "Active guests retrieved successfully", guests);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving active guests: {ex.Message}");
            }
        }

        [HttpGet("nationality/{nationality}")]
        public async Task<IActionResult> GetGuestsByNationality(string nationality)
        {
            try
            {
                var guests = await _guestService.GetGuestsByNationalityAsync(nationality);
                return this.CreateResponse(200, $"Guests from {nationality} retrieved successfully", guests);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving guests by nationality: {ex.Message}");
            }
        }

        [HttpGet("top-spending")]
        public async Task<IActionResult> GetTopSpendingGuests([FromQuery] int count = 10)
        {
            try
            {
                var guests = await _guestService.GetTopSpendingGuestsAsync(count);
                return this.CreateResponse(200, $"Top {count} spending guests retrieved successfully", guests);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving top spending guests: {ex.Message}");
            }
        }

        [HttpGet("frequent")]
        public async Task<IActionResult> GetFrequentGuests([FromQuery] int minStays = 3)
        {
            try
            {
                var guests = await _guestService.GetFrequentGuestsAsync(minStays);
                return this.CreateResponse(200, $"Frequent guests with {minStays}+ stays retrieved successfully", guests);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving frequent guests: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateGuest([FromBody] GuestCreateDto guestCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid guest data", ModelState);
                }

                var guest = await _guestService.CreateGuestAsync(guestCreateDto);
                return this.CreateResponse(201, "Guest created successfully", guest);
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error creating guest: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateGuest(int id, [FromBody] GuestUpdateDto guestUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid guest data", ModelState);
                }

                guestUpdateDto.Id = id;
                var guest = await _guestService.UpdateGuestAsync(guestUpdateDto);
                if (guest == null)
                {
                    return this.CreateResponse(404, "Guest not found");
                }
                return this.CreateResponse(200, "Guest updated successfully", guest);
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error updating guest: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            try
            {
                var result = await _guestService.DeleteGuestAsync(id);
                if (!result)
                {
                    return this.CreateResponse(404, "Guest not found");
                }
                return this.CreateResponse(200, "Guest deleted successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error deleting guest: {ex.Message}");
            }
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeactivateGuest(int id)
        {
            try
            {
                var result = await _guestService.DeactivateGuestAsync(id);
                if (!result)
                {
                    return this.CreateResponse(404, "Guest not found");
                }
                return this.CreateResponse(200, "Guest deactivated successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error deactivating guest: {ex.Message}");
            }
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ActivateGuest(int id)
        {
            try
            {
                var result = await _guestService.ActivateGuestAsync(id);
                if (!result)
                {
                    return this.CreateResponse(404, "Guest not found");
                }
                return this.CreateResponse(200, "Guest activated successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error activating guest: {ex.Message}");
            }
        }
    }
}
