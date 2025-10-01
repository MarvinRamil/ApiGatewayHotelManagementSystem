using HootelManagementDomain.enums;
using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Services;
using Microsoft.AspNetCore.Mvc;
using BookingServiceApi.Extensions;

namespace BookingServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceDateController : ControllerBase
    {
        private readonly IMaintenanceDateService _maintenanceDateService;

        public MaintenanceDateController(IMaintenanceDateService maintenanceDateService)
        {
            _maintenanceDateService = maintenanceDateService;
        }

        /// <summary>
        /// Create a new maintenance date
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMaintenanceDate([FromBody] MaintenanceDateCreateDto createDto)
        {
            var result = await _maintenanceDateService.CreateMaintenanceDateAsync(createDto);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Get maintenance date by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceDate(int id)
        {
            var result = await _maintenanceDateService.GetMaintenanceDateByIdAsync(id);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Get all maintenance dates
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMaintenanceDates()
        {
            var result = await _maintenanceDateService.GetAllMaintenanceDatesAsync();
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Update maintenance date
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenanceDate(int id, [FromBody] MaintenanceDateUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return this.CreateResponse(400, "ID mismatch");
            }

            var result = await _maintenanceDateService.UpdateMaintenanceDateAsync(updateDto);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Delete maintenance date
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenanceDate(int id)
        {
            var result = await _maintenanceDateService.DeleteMaintenanceDateAsync(id);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Update maintenance date status
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateMaintenanceDateStatus(int id, [FromBody] MaintenanceDateStatusUpdateDto statusUpdateDto)
        {
            var result = await _maintenanceDateService.UpdateMaintenanceDateStatusAsync(id, statusUpdateDto);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Get maintenance dates by room ID
        /// </summary>
        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetMaintenanceDatesByRoomId(int roomId)
        {
            var result = await _maintenanceDateService.GetMaintenanceDatesByRoomIdAsync(roomId);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Get maintenance dates by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetMaintenanceDatesByStatus(MaintenanceStatus status)
        {
            var result = await _maintenanceDateService.GetMaintenanceDatesByStatusAsync(status);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Get maintenance dates by date range
        /// </summary>
        [HttpGet("date-range")]
        public async Task<IActionResult> GetMaintenanceDatesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var result = await _maintenanceDateService.GetMaintenanceDatesByDateRangeAsync(startDate, endDate);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Get upcoming maintenance dates
        /// </summary>
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingMaintenanceDates()
        {
            var result = await _maintenanceDateService.GetUpcomingMaintenanceDatesAsync();
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Get maintenance dates for a specific date
        /// </summary>
        [HttpGet("date/{date}")]
        public async Task<IActionResult> GetMaintenanceDatesForDate(DateTime date)
        {
            var result = await _maintenanceDateService.GetMaintenanceDatesForDateAsync(date);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Get overdue maintenance dates
        /// </summary>
        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdueMaintenanceDates()
        {
            var result = await _maintenanceDateService.GetOverdueMaintenanceDatesAsync();
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Get maintenance dates by room and date range
        /// </summary>
        [HttpGet("room/{roomId}/date-range")]
        public async Task<IActionResult> GetMaintenanceDatesByRoomAndDateRange(
            int roomId,
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var result = await _maintenanceDateService.GetMaintenanceDatesByRoomAndDateRangeAsync(roomId, startDate, endDate);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Check for maintenance conflicts
        /// </summary>
        [HttpGet("check-conflict")]
        public async Task<IActionResult> CheckMaintenanceConflict(
            [FromQuery] int roomId, 
            [FromQuery] DateTime date, 
            [FromQuery] int? excludeId = null)
        {
            var result = await _maintenanceDateService.CheckMaintenanceConflictAsync(roomId, date, excludeId);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Complete maintenance
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteMaintenance(int id, [FromBody] CompleteMaintenanceDto completeDto)
        {
            var result = await _maintenanceDateService.CompleteMaintenanceAsync(id, completeDto.CompletedBy);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }

        /// <summary>
        /// Cancel maintenance
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelMaintenance(int id, [FromBody] CancelMaintenanceDto cancelDto)
        {
            var result = await _maintenanceDateService.CancelMaintenanceAsync(id, cancelDto.Reason);
            return this.CreateResponse(result.StatusCode, result.Message, result.Data);
        }
    }

    public class CompleteMaintenanceDto
    {
        public string CompletedBy { get; set; } = string.Empty;
    }

    public class CancelMaintenanceDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}