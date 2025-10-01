using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Services;
using HootelManagementDomain.enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingServiceApi.Extensions;

namespace BookManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelStaffController : ControllerBase
    {
        private readonly IHotelStaffService _hotelStaffService;

        public HotelStaffController(IHotelStaffService hotelStaffService)
        {
            _hotelStaffService = hotelStaffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            try
            {
                var staff = await _hotelStaffService.GetAllStaffAsync();
                return this.CreateResponse(200, "Staff retrieved successfully", staff);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving staff: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            try
            {
                var staff = await _hotelStaffService.GetStaffByIdAsync(id);
                if (staff == null)
                {
                    return this.CreateResponse(404, "Staff not found");
                }
                return this.CreateResponse(200, "Staff retrieved successfully", staff);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving staff: {ex.Message}");
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetStaffByEmail(string email)
        {
            try
            {
                var staff = await _hotelStaffService.GetStaffByEmailAsync(email);
                if (staff == null)
                {
                    return this.CreateResponse(404, "Staff not found");
                }
                return this.CreateResponse(200, "Staff retrieved successfully", staff);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving staff: {ex.Message}");
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveStaff()
        {
            try
            {
                var staff = await _hotelStaffService.GetActiveStaffAsync();
                return this.CreateResponse(200, "Active staff retrieved successfully", staff);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving active staff: {ex.Message}");
            }
        }

        [HttpGet("role/{role}")]
        public async Task<IActionResult> GetStaffByRole(StaffRoles role)
        {
            try
            {
                var staff = await _hotelStaffService.GetStaffByRoleAsync(role);
                return this.CreateResponse(200, $"Staff with role {role} retrieved successfully", staff);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving staff by role: {ex.Message}");
            }
        }

        [HttpGet("department/{department}")]
        public async Task<IActionResult> GetStaffByDepartment(string department)
        {
            try
            {
                var staff = await _hotelStaffService.GetStaffByDepartmentAsync(department);
                return this.CreateResponse(200, $"Staff in department {department} retrieved successfully", staff);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving staff by department: {ex.Message}");
            }
        }

        [HttpGet("salary-range")]
        public async Task<IActionResult> GetStaffBySalaryRange([FromQuery] decimal minSalary, [FromQuery] decimal maxSalary)
        {
            try
            {
                var staff = await _hotelStaffService.GetStaffBySalaryRangeAsync(minSalary, maxSalary);
                return this.CreateResponse(200, $"Staff with salary between {minSalary} and {maxSalary} retrieved successfully", staff);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving staff by salary range: {ex.Message}");
            }
        }

        [HttpGet("hired-after")]
        public async Task<IActionResult> GetStaffHiredAfter([FromQuery] DateTime hireDate)
        {
            try
            {
                var staff = await _hotelStaffService.GetStaffHiredAfterAsync(hireDate);
                return this.CreateResponse(200, $"Staff hired after {hireDate:yyyy-MM-dd} retrieved successfully", staff);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving staff hired after date: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStaff([FromBody] HotelStaffCreateDto staffCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid staff data", ModelState);
                }

                var staff = await _hotelStaffService.CreateStaffAsync(staffCreateDto);
                return this.CreateResponse(201, "Staff created successfully", staff);
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error creating staff: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] HotelStaffUpdateDto staffUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.CreateResponse(400, "Invalid staff data", ModelState);
                }

                staffUpdateDto.Id = id;
                var staff = await _hotelStaffService.UpdateStaffAsync(staffUpdateDto);
                if (staff == null)
                {
                    return this.CreateResponse(404, "Staff not found");
                }
                return this.CreateResponse(200, "Staff updated successfully", staff);
            }
            catch (InvalidOperationException ex)
            {
                return this.CreateResponse(400, ex.Message);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error updating staff: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            try
            {
                var result = await _hotelStaffService.DeleteStaffAsync(id);
                if (!result)
                {
                    return this.CreateResponse(404, "Staff not found");
                }
                return this.CreateResponse(200, "Staff deactivated successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error deleting staff: {ex.Message}");
            }
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeactivateStaff(int id)
        {
            try
            {
                var result = await _hotelStaffService.DeactivateStaffAsync(id);
                if (!result)
                {
                    return this.CreateResponse(404, "Staff not found");
                }
                return this.CreateResponse(200, "Staff deactivated successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error deactivating staff: {ex.Message}");
            }
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ActivateStaff(int id)
        {
            try
            {
                var result = await _hotelStaffService.ActivateStaffAsync(id);
                if (!result)
                {
                    return this.CreateResponse(404, "Staff not found");
                }
                return this.CreateResponse(200, "Staff activated successfully");
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error activating staff: {ex.Message}");
            }
        }
    }
}
