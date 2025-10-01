using HootelManagementDomain.enums;
using HotelManagementApplication.Dto;

namespace HotelManagementApplication.Interface.Services
{
    public interface IMaintenanceDateService
    {
        Task<ApiResponse<MaintenanceDateDto>> CreateMaintenanceDateAsync(MaintenanceDateCreateDto createDto);
        Task<ApiResponse<MaintenanceDateDto>> GetMaintenanceDateByIdAsync(int id);
        Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetAllMaintenanceDatesAsync();
        Task<ApiResponse<MaintenanceDateDto>> UpdateMaintenanceDateAsync(MaintenanceDateUpdateDto updateDto);
        Task<ApiResponse<bool>> DeleteMaintenanceDateAsync(int id);
        Task<ApiResponse<MaintenanceDateDto>> UpdateMaintenanceDateStatusAsync(int id, MaintenanceDateStatusUpdateDto statusUpdateDto);
        
        // Query methods
        Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesByRoomIdAsync(int roomId);
        Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesByStatusAsync(MaintenanceStatus status);
        Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetUpcomingMaintenanceDatesAsync();
        Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesForDateAsync(DateTime date);
        Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetOverdueMaintenanceDatesAsync();
        Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesByRoomAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate);
        
        // Utility methods
        Task<ApiResponse<bool>> CheckMaintenanceConflictAsync(int roomId, DateTime date, int? excludeId = null);
        Task<ApiResponse<bool>> CompleteMaintenanceAsync(int id, string completedBy);
        Task<ApiResponse<bool>> CancelMaintenanceAsync(int id, string reason);
    }
}
