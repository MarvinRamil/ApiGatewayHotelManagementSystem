using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using HotelManagementApplication.Dto;

namespace HotelManagementApplication.Interface.Repositories
{
    public interface IMaintenanceDate : IGeneric<MaintenanceDate>
    {
        Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesByRoomIdAsync(int roomId);
        Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesByStatusAsync(MaintenanceStatus status);
        Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MaintenanceDateDto>> GetUpcomingMaintenanceDatesAsync();
        Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesForDateAsync(DateTime date);
        Task<IEnumerable<MaintenanceDateDto>> GetOverdueMaintenanceDatesAsync();
        Task<MaintenanceDateDto?> GetMaintenanceDateByIdAsync(int id);
        Task<bool> HasMaintenanceConflictAsync(int roomId, DateTime date, int? excludeId = null);
        Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesByRoomAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate);
    }
}
