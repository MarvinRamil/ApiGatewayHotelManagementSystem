using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingServiceApi.Extensions;

namespace BookingServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for dashboard access
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Get complete dashboard data
        /// </summary>
        /// <param name="filter">Dashboard filter options</param>
        /// <returns>Complete dashboard data including stats, revenue, room status, and recent bookings</returns>
        [HttpGet]
        public async Task<IActionResult> GetDashboard([FromQuery] DashboardFilterDto? filter)
        {
            try
            {
                var result = await _dashboardService.GetDashboardDataAsync(filter);
                return this.CreateResponse(result.Success ? 200 : 400, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data");
                return this.CreateResponse(500, $"Error retrieving dashboard data: {ex.Message}");
            }
        }

        /// <summary>
        /// Get hotel statistics
        /// </summary>
        /// <returns>Hotel statistics including room counts, revenue, and occupancy</returns>
        [HttpGet("stats")]
        public async Task<IActionResult> GetHotelStats()
        {
            try
            {
                var result = await _dashboardService.GetHotelStatsAsync();
                return this.CreateResponse(result.Success ? 200 : 400, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotel statistics");
                return this.CreateResponse(500, $"Error retrieving hotel statistics: {ex.Message}");
            }
        }

        /// <summary>
        /// Get revenue data for charts
        /// </summary>
        /// <param name="period">Time period for revenue data (week, month, quarter, year)</param>
        /// <returns>Revenue data for the specified period</returns>
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueData([FromQuery] string period = "month")
        {
            try
            {
                if (!IsValidPeriod(period))
                {
                    return this.CreateResponse(400, "Invalid period. Valid periods are: week, month, quarter, year");
                }

                var result = await _dashboardService.GetRevenueDataAsync(period);
                return this.CreateResponse(result.Success ? 200 : 400, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving revenue data");
                return this.CreateResponse(500, $"Error retrieving revenue data: {ex.Message}");
            }
        }

        /// <summary>
        /// Get room status overview
        /// </summary>
        /// <returns>Room status distribution with counts and colors</returns>
        [HttpGet("room-status")]
        public async Task<IActionResult> GetRoomStatusOverview()
        {
            try
            {
                var result = await _dashboardService.GetRoomStatusOverviewAsync();
                return this.CreateResponse(result.Success ? 200 : 400, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving room status overview");
                return this.CreateResponse(500, $"Error retrieving room status overview: {ex.Message}");
            }
        }

        /// <summary>
        /// Get recent bookings
        /// </summary>
        /// <param name="limit">Number of recent bookings to retrieve (default: 5)</param>
        /// <returns>List of recent bookings</returns>
        [HttpGet("recent-bookings")]
        public async Task<IActionResult> GetRecentBookings([FromQuery] int limit = 5)
        {
            try
            {
                if (limit <= 0 || limit > 50)
                {
                    return this.CreateResponse(400, "Limit must be between 1 and 50");
                }

                var result = await _dashboardService.GetRecentBookingsAsync(limit);
                return this.CreateResponse(result.Success ? 200 : 400, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent bookings");
                return this.CreateResponse(500, $"Error retrieving recent bookings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get quick actions for dashboard
        /// </summary>
        /// <returns>List of quick actions available on the dashboard</returns>
        [HttpGet("quick-actions")]
        public async Task<IActionResult> GetQuickActions()
        {
            try
            {
                var result = await _dashboardService.GetQuickActionsAsync();
                return this.CreateResponse(result.Success ? 200 : 400, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quick actions");
                return this.CreateResponse(500, $"Error retrieving quick actions: {ex.Message}");
            }
        }

        /// <summary>
        /// Get stat cards for dashboard
        /// </summary>
        /// <returns>List of stat cards with metrics and trends</returns>
        [HttpGet("stat-cards")]
        public async Task<IActionResult> GetStatCards()
        {
            try
            {
                var result = await _dashboardService.GetStatCardsAsync();
                return this.CreateResponse(result.Success ? 200 : 400, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stat cards");
                return this.CreateResponse(500, $"Error retrieving stat cards: {ex.Message}");
            }
        }

        /// <summary>
        /// Get dashboard summary for mobile/quick view
        /// </summary>
        /// <returns>Summary dashboard data with key metrics</returns>
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                var filter = new DashboardFilterDto
                {
                    RecentBookingsLimit = 3,
                    IncludeRevenueData = false,
                    IncludeRoomStatus = true
                };

                var result = await _dashboardService.GetDashboardDataAsync(filter);
                return this.CreateResponse(result.Success ? 200 : 400, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard summary");
                return this.CreateResponse(500, $"Error retrieving dashboard summary: {ex.Message}");
            }
        }

        private static bool IsValidPeriod(string period)
        {
            var validPeriods = new[] { "week", "month", "quarter", "year" };
            return validPeriods.Contains(period.ToLower());
        }
    }
}
