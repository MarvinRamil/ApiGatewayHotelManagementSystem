using System.ComponentModel.DataAnnotations;

namespace HotelManagementApplication.Dto
{
    public class DashboardDto
    {
        public HotelStatsDto Stats { get; set; } = new HotelStatsDto();
        public List<RevenueDataDto> RevenueData { get; set; } = new List<RevenueDataDto>();
        public List<RoomStatusDataDto> RoomStatusOverview { get; set; } = new List<RoomStatusDataDto>();
        public List<RecentBookingDto> RecentBookings { get; set; } = new List<RecentBookingDto>();
        public List<QuickActionDto> QuickActions { get; set; } = new List<QuickActionDto>();
    }

    // Hotel Statistics for Dashboard
    public class HotelStatsDto
    {
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int OutOfOrderRooms { get; set; }
        public int TodayCheckIns { get; set; }
        public int TodayCheckOuts { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal AverageRate { get; set; }
        public int OccupancyRate { get; set; }
    }

    // Revenue Chart Data
    public class RevenueDataDto
    {
        [Required]
        [StringLength(10)]
        public string Month { get; set; } = string.Empty; // "Jan", "Feb", "Mar", etc.
        
        public decimal Revenue { get; set; }
        
        public int Bookings { get; set; }
        
        public int Occupancy { get; set; }
    }

    // Room Status Overview Data
    public class RoomStatusDataDto
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty; // "Available", "Occupied", "Cleaning", "Maintenance", "Out of Order"
        
        public int Count { get; set; }
        
        [Required]
        [StringLength(7)]
        public string Color { get; set; } = string.Empty; // Hex color like "#10B981"
    }

    // Recent Bookings for Dashboard
    public class RecentBookingDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string BookingNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string RoomType { get; set; } = string.Empty;
        
        [Required]
        public DateTime CheckIn { get; set; }
        
        [Required]
        public DateTime CheckOut { get; set; }
        
        public int Status { get; set; } // 0 = confirmed, 1 = checked-in, 2 = checked-out, etc.
        
        public decimal TotalAmount { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }

    // Quick Actions for Dashboard
    public class QuickActionDto
    {
        [Required]
        [StringLength(50)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Value { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Description { get; set; }
        
        [StringLength(20)]
        public string? Icon { get; set; }
        
        [StringLength(20)]
        public string? Color { get; set; }
    }

    // Dashboard Stat Card Data
    public class StatCardDto
    {
        [Required]
        [StringLength(50)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Value { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Change { get; set; }
        
        [StringLength(20)]
        public string? ChangeType { get; set; } // "increase", "decrease", "neutral"
        
        [StringLength(20)]
        public string? Icon { get; set; }
        
        [StringLength(20)]
        public string? Color { get; set; }
    }

    // Dashboard API Response
    public class DashboardResponseDto
    {
        public DashboardDto Data { get; set; } = new DashboardDto();
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    // Dashboard Filter for API calls
    public class DashboardFilterDto
    {
        public string? Period { get; set; } = "month"; // "week", "month", "quarter", "year"
        public int RecentBookingsLimit { get; set; } = 5;
        public bool IncludeRevenueData { get; set; } = true;
        public bool IncludeRoomStatus { get; set; } = true;
    }
}
