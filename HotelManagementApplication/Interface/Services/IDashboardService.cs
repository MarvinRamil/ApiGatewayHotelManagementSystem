using HotelManagementApplication.Dto;

namespace HotelManagementApplication.Interface.Services
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardDto>> GetDashboardDataAsync(DashboardFilterDto? filter = null);
        Task<ApiResponse<HotelStatsDto>> GetHotelStatsAsync();
        Task<ApiResponse<List<RevenueDataDto>>> GetRevenueDataAsync(string period = "month");
        Task<ApiResponse<List<RoomStatusDataDto>>> GetRoomStatusOverviewAsync();
        Task<ApiResponse<List<RecentBookingDto>>> GetRecentBookingsAsync(int limit = 5);
        Task<ApiResponse<List<QuickActionDto>>> GetQuickActionsAsync();
        Task<ApiResponse<List<StatCardDto>>> GetStatCardsAsync();
    }
}
