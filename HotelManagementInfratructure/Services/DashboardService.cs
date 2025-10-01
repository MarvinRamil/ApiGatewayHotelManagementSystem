using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementApplication.Interface.Services;
using HootelManagementDomain.enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagementInfratructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(IUnitOfWork unitOfWork, ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse<DashboardDto>> GetDashboardDataAsync(DashboardFilterDto? filter = null)
        {
            try
            {
                filter ??= new DashboardFilterDto();

                var dashboardData = new DashboardDto();

                // Get hotel statistics
                var statsResult = await GetHotelStatsAsync();
                if (statsResult.Success)
                {
                    dashboardData.Stats = statsResult.Data!;
                }

                // Get revenue data if requested
                if (filter.IncludeRevenueData)
                {
                    var revenueResult = await GetRevenueDataAsync(filter.Period ?? "month");
                    if (revenueResult.Success)
                    {
                        dashboardData.RevenueData = revenueResult.Data!;
                    }
                }

                // Get room status overview if requested
                if (filter.IncludeRoomStatus)
                {
                    var roomStatusResult = await GetRoomStatusOverviewAsync();
                    if (roomStatusResult.Success)
                    {
                        dashboardData.RoomStatusOverview = roomStatusResult.Data!;
                    }
                }

                // Get recent bookings
                var recentBookingsResult = await GetRecentBookingsAsync(filter.RecentBookingsLimit);
                if (recentBookingsResult.Success)
                {
                    dashboardData.RecentBookings = recentBookingsResult.Data!;
                }

                // Get quick actions
                var quickActionsResult = await GetQuickActionsAsync();
                if (quickActionsResult.Success)
                {
                    dashboardData.QuickActions = quickActionsResult.Data!;
                }

                _logger.LogInformation("Dashboard data retrieved successfully");
                return ResponseHelper.Success(dashboardData, "Dashboard data retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard data");
                return ResponseHelper.Failure<DashboardDto>($"Error retrieving dashboard data: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<HotelStatsDto>> GetHotelStatsAsync()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                // Get room statistics
                var totalRooms = await _unitOfWork.Room.GetAllRooms().CountAsync();
                var occupiedRooms = await _unitOfWork.Room.GetAllRooms()
                    .CountAsync(r => r.Status == RoomStatus.Occupied);
                var availableRooms = await _unitOfWork.Room.GetAllRooms()
                    .CountAsync(r => r.Status == RoomStatus.Available);
                var outOfOrderRooms = await _unitOfWork.Room.GetAllRooms()
                    .CountAsync(r => r.Status == RoomStatus.OutOfOrder);

                // Get today's check-ins and check-outs
                var todayCheckIns = await _unitOfWork.Booking.GetAll()
                    .CountAsync(b => b.CheckIn.Date == today && b.Status == BookingStatus.Confirmed);
                var todayCheckOuts = await _unitOfWork.Booking.GetAll()
                    .CountAsync(b => b.CheckOut.Date == today && b.Status == BookingStatus.CheckedIn);

                // Get today's revenue
                var todayRevenue = await _unitOfWork.Booking.GetAll()
                    .Where(b => b.CheckIn.Date == today && b.Status == BookingStatus.Confirmed)
                    .SumAsync(b => b.TotalAmount);

                // Get monthly revenue
                var monthlyRevenue = await _unitOfWork.Booking.GetAll()
                    .Where(b => b.CheckIn >= monthStart && b.CheckIn <= monthEnd && 
                               (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckedIn || b.Status == BookingStatus.CheckedOut))
                    .SumAsync(b => b.TotalAmount);

                // Calculate average rate
                var totalBookings = await _unitOfWork.Booking.GetAll()
                    .Where(b => b.CheckIn >= monthStart && b.CheckIn <= monthEnd)
                    .CountAsync();
                var averageRate = totalBookings > 0 ? monthlyRevenue / totalBookings : 0;

                // Calculate occupancy rate
                var occupancyRate = totalRooms > 0 ? (int)((occupiedRooms * 100.0) / totalRooms) : 0;

                var stats = new HotelStatsDto
                {
                    TotalRooms = totalRooms,
                    OccupiedRooms = occupiedRooms,
                    AvailableRooms = availableRooms,
                    OutOfOrderRooms = outOfOrderRooms,
                    TodayCheckIns = todayCheckIns,
                    TodayCheckOuts = todayCheckOuts,
                    TodayRevenue = todayRevenue,
                    MonthlyRevenue = monthlyRevenue,
                    AverageRate = averageRate,
                    OccupancyRate = occupancyRate
                };

                _logger.LogInformation("Hotel statistics retrieved successfully");
                return ResponseHelper.Success(stats, "Hotel statistics retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotel statistics");
                return ResponseHelper.Failure<HotelStatsDto>($"Error retrieving hotel statistics: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<List<RevenueDataDto>>> GetRevenueDataAsync(string period = "month")
        {
            try
            {
                var revenueData = new List<RevenueDataDto>();
                var currentDate = DateTime.UtcNow;
                var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

                // Get data for the last 12 months
                for (int i = 11; i >= 0; i--)
                {
                    var monthDate = currentDate.AddMonths(-i);
                    var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    var monthRevenue = await _unitOfWork.Booking.GetAll()
                        .Where(b => b.CheckIn >= monthStart && b.CheckIn <= monthEnd && 
                                   (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckedIn || b.Status == BookingStatus.CheckedOut))
                        .SumAsync(b => b.TotalAmount);

                    var monthBookings = await _unitOfWork.Booking.GetAll()
                        .CountAsync(b => b.CheckIn >= monthStart && b.CheckIn <= monthEnd);

                    // Calculate occupancy for the month (simplified)
                    var monthOccupancy = await _unitOfWork.Booking.GetAll()
                        .Where(b => b.CheckIn <= monthEnd && b.CheckOut >= monthStart)
                        .CountAsync();

                    revenueData.Add(new RevenueDataDto
                    {
                        Month = months[monthDate.Month - 1],
                        Revenue = monthRevenue,
                        Bookings = monthBookings,
                        Occupancy = monthOccupancy
                    });
                }

                _logger.LogInformation("Revenue data retrieved successfully");
                return ResponseHelper.Success(revenueData, "Revenue data retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving revenue data");
                return ResponseHelper.Failure<List<RevenueDataDto>>($"Error retrieving revenue data: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<List<RoomStatusDataDto>>> GetRoomStatusOverviewAsync()
        {
            try
            {
                var roomStatusData = new List<RoomStatusDataDto>();

                // Get room counts by status
                var availableCount = await _unitOfWork.Room.GetAllRooms()
                    .CountAsync(r => r.Status == RoomStatus.Available);
                var occupiedCount = await _unitOfWork.Room.GetAllRooms()
                    .CountAsync(r => r.Status == RoomStatus.Occupied);
                var cleaningCount = await _unitOfWork.Room.GetAllRooms()
                    .CountAsync(r => r.Status == RoomStatus.Cleaning);
                var maintenanceCount = await _unitOfWork.Room.GetAllRooms()
                    .CountAsync(r => r.Status == RoomStatus.Maintenance);
                var outOfOrderCount = await _unitOfWork.Room.GetAllRooms()
                    .CountAsync(r => r.Status == RoomStatus.OutOfOrder);

                roomStatusData.AddRange(new[]
                {
                    new RoomStatusDataDto { Status = "Available", Count = availableCount, Color = "#10B981" },
                    new RoomStatusDataDto { Status = "Occupied", Count = occupiedCount, Color = "#3B82F6" },
                    new RoomStatusDataDto { Status = "Cleaning", Count = cleaningCount, Color = "#F59E0B" },
                    new RoomStatusDataDto { Status = "Maintenance", Count = maintenanceCount, Color = "#EF4444" },
                    new RoomStatusDataDto { Status = "Out of Order", Count = outOfOrderCount, Color = "#6B7280" }
                });

                _logger.LogInformation("Room status overview retrieved successfully");
                return ResponseHelper.Success(roomStatusData, "Room status overview retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving room status overview");
                return ResponseHelper.Failure<List<RoomStatusDataDto>>($"Error retrieving room status overview: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<List<RecentBookingDto>>> GetRecentBookingsAsync(int limit = 5)
        {
            try
            {
                var recentBookings = await _unitOfWork.Booking.GetAll()
                    .Include(b => b.Room)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(limit)
                    .Select(b => new RecentBookingDto
                    {
                        Id = b.Id,
                        BookingNumber = b.BookingNumber,
                        RoomNumber = b.Room.Number,
                        RoomType = b.Room.Type,
                        CheckIn = b.CheckIn,
                        CheckOut = b.CheckOut,
                        Status = (int)b.Status,
                        TotalAmount = b.TotalAmount,
                        CreatedAt = b.CreatedAt
                    })
                    .ToListAsync();

                _logger.LogInformation("Recent bookings retrieved successfully");
                return ResponseHelper.Success(recentBookings, "Recent bookings retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent bookings");
                return ResponseHelper.Failure<List<RecentBookingDto>>($"Error retrieving recent bookings: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<List<QuickActionDto>>> GetQuickActionsAsync()
        {
            try
            {
                var quickActions = new List<QuickActionDto>
                {
                    new QuickActionDto
                    {
                        Title = "New Booking",
                        Value = "Create",
                        Description = "Create a new booking for a guest",
                        Icon = "plus",
                        Color = "blue"
                    },
                    new QuickActionDto
                    {
                        Title = "Check In",
                        Value = "CheckIn",
                        Description = "Check in arriving guests",
                        Icon = "login",
                        Color = "green"
                    },
                    new QuickActionDto
                    {
                        Title = "Check Out",
                        Value = "CheckOut",
                        Description = "Check out departing guests",
                        Icon = "logout",
                        Color = "orange"
                    },
                    new QuickActionDto
                    {
                        Title = "Room Status",
                        Value = "RoomStatus",
                        Description = "Update room status",
                        Icon = "home",
                        Color = "purple"
                    },
                    new QuickActionDto
                    {
                        Title = "Maintenance",
                        Value = "Maintenance",
                        Description = "Schedule room maintenance",
                        Icon = "wrench",
                        Color = "red"
                    },
                    new QuickActionDto
                    {
                        Title = "Reports",
                        Value = "Reports",
                        Description = "Generate hotel reports",
                        Icon = "chart",
                        Color = "indigo"
                    }
                };

                _logger.LogInformation("Quick actions retrieved successfully");
                return ResponseHelper.Success(quickActions, "Quick actions retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quick actions");
                return ResponseHelper.Failure<List<QuickActionDto>>($"Error retrieving quick actions: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<List<StatCardDto>>> GetStatCardsAsync()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);
                var thisMonth = new DateTime(today.Year, today.Month, 1);
                var lastMonth = thisMonth.AddMonths(-1);

                // Get today's revenue
                var todayRevenue = await _unitOfWork.Booking.GetAll()
                    .Where(b => b.CheckIn.Date == today && b.Status == BookingStatus.Confirmed)
                    .SumAsync(b => b.TotalAmount);

                // Get yesterday's revenue for comparison
                var yesterdayRevenue = await _unitOfWork.Booking.GetAll()
                    .Where(b => b.CheckIn.Date == yesterday && b.Status == BookingStatus.Confirmed)
                    .SumAsync(b => b.TotalAmount);

                // Get this month's bookings
                var thisMonthBookings = await _unitOfWork.Booking.GetAll()
                    .CountAsync(b => b.CheckIn >= thisMonth && b.CheckIn < thisMonth.AddMonths(1));

                // Get last month's bookings for comparison
                var lastMonthBookings = await _unitOfWork.Booking.GetAll()
                    .CountAsync(b => b.CheckIn >= lastMonth && b.CheckIn < thisMonth);

                // Get occupancy rate
                var totalRooms = await _unitOfWork.Room.GetAllRooms().CountAsync();
                var occupiedRooms = await _unitOfWork.Room.GetAllRooms()
                    .CountAsync(r => r.Status == RoomStatus.Occupied);
                var occupancyRate = totalRooms > 0 ? (int)((occupiedRooms * 100.0) / totalRooms) : 0;

                // Get average daily rate
                var avgDailyRate = await _unitOfWork.Booking.GetAll()
                    .Where(b => b.CheckIn >= thisMonth && b.CheckIn < thisMonth.AddMonths(1))
                    .AverageAsync(b => b.TotalAmount);

                var statCards = new List<StatCardDto>
                {
                    new StatCardDto
                    {
                        Title = "Today's Revenue",
                        Value = $"${todayRevenue:F2}",
                        Change = yesterdayRevenue > 0 ? $"{((todayRevenue - yesterdayRevenue) / yesterdayRevenue * 100):F1}%" : "0%",
                        ChangeType = todayRevenue >= yesterdayRevenue ? "increase" : "decrease",
                        Icon = "dollar",
                        Color = "green"
                    },
                    new StatCardDto
                    {
                        Title = "Monthly Bookings",
                        Value = thisMonthBookings.ToString(),
                        Change = lastMonthBookings > 0 ? $"{((thisMonthBookings - lastMonthBookings) / (double)lastMonthBookings * 100):F1}%" : "0%",
                        ChangeType = thisMonthBookings >= lastMonthBookings ? "increase" : "decrease",
                        Icon = "calendar",
                        Color = "blue"
                    },
                    new StatCardDto
                    {
                        Title = "Occupancy Rate",
                        Value = $"{occupancyRate}%",
                        Change = "0%",
                        ChangeType = "neutral",
                        Icon = "users",
                        Color = "purple"
                    },
                    new StatCardDto
                    {
                        Title = "Average Daily Rate",
                        Value = $"${avgDailyRate:F2}",
                        Change = "0%",
                        ChangeType = "neutral",
                        Icon = "trending-up",
                        Color = "orange"
                    }
                };

                _logger.LogInformation("Stat cards retrieved successfully");
                return ResponseHelper.Success(statCards, "Stat cards retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stat cards");
                return ResponseHelper.Failure<List<StatCardDto>>($"Error retrieving stat cards: {ex.Message}", 500);
            }
        }
    }
}
