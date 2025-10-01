using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementInfratructure.Interface;
using HotelManagementInfratructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementInfratructure.Repository
{
    public class MaintenanceDateRepository : GenericRepository<MaintenanceDate>, IMaintenanceDate
    {
        public MaintenanceDateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesByRoomIdAsync(int roomId)
        {
            var maintenanceDates = await _context.MaintenanceDates
                .Include(m => m.Room)
                .Where(m => m.RoomId == roomId)
                .OrderBy(m => m.Date)
                .ToListAsync();

            return maintenanceDates.Select(MapToDto);
        }

        public async Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesByStatusAsync(MaintenanceStatus status)
        {
            var maintenanceDates = await _context.MaintenanceDates
                .Include(m => m.Room)
                .Where(m => m.Status == status)
                .OrderBy(m => m.Date)
                .ToListAsync();

            return maintenanceDates.Select(MapToDto);
        }

        public async Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var maintenanceDates = await _context.MaintenanceDates
                .Include(m => m.Room)
                .Where(m => m.Date >= startDate && m.Date <= endDate)
                .OrderBy(m => m.Date)
                .ToListAsync();

            return maintenanceDates.Select(MapToDto);
        }

        public async Task<IEnumerable<MaintenanceDateDto>> GetUpcomingMaintenanceDatesAsync()
        {
            var today = DateTime.Today;
            var maintenanceDates = await _context.MaintenanceDates
                .Include(m => m.Room)
                .Where(m => m.Date >= today && m.Status != MaintenanceStatus.Completed && m.Status != MaintenanceStatus.Cancelled)
                .OrderBy(m => m.Date)
                .ToListAsync();

            return maintenanceDates.Select(MapToDto);
        }

        public async Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesForDateAsync(DateTime date)
        {
            var maintenanceDates = await _context.MaintenanceDates
                .Include(m => m.Room)
                .Where(m => m.Date.Date == date.Date)
                .OrderBy(m => m.Date)
                .ToListAsync();

            return maintenanceDates.Select(MapToDto);
        }

        public async Task<IEnumerable<MaintenanceDateDto>> GetOverdueMaintenanceDatesAsync()
        {
            var today = DateTime.Today;
            var maintenanceDates = await _context.MaintenanceDates
                .Include(m => m.Room)
                .Where(m => m.Date < today && m.Status != MaintenanceStatus.Completed && m.Status != MaintenanceStatus.Cancelled)
                .OrderBy(m => m.Date)
                .ToListAsync();

            return maintenanceDates.Select(MapToDto);
        }

        public async Task<MaintenanceDateDto?> GetMaintenanceDateByIdAsync(int id)
        {
            var maintenanceDate = await _context.MaintenanceDates
                .Include(m => m.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            return maintenanceDate != null ? MapToDto(maintenanceDate) : null;
        }

        public async Task<bool> HasMaintenanceConflictAsync(int roomId, DateTime date, int? excludeId = null)
        {
            var query = _context.MaintenanceDates
                .Where(m => m.RoomId == roomId && m.Date.Date == date.Date);

            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<MaintenanceDateDto>> GetMaintenanceDatesByRoomAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate)
        {
            var maintenanceDates = await _context.MaintenanceDates
                .Include(m => m.Room)
                .Where(m => m.RoomId == roomId && m.Date >= startDate && m.Date <= endDate)
                .OrderBy(m => m.Date)
                .ToListAsync();

            return maintenanceDates.Select(MapToDto);
        }

        private static MaintenanceDateDto MapToDto(MaintenanceDate maintenanceDate)
        {
            return new MaintenanceDateDto
            {
                Id = maintenanceDate.Id,
                Date = maintenanceDate.Date,
                Status = maintenanceDate.Status,
                RoomId = maintenanceDate.RoomId,
                Description = maintenanceDate.Description,
                Notes = maintenanceDate.Notes,
                CompletedAt = maintenanceDate.CompletedAt,
                CompletedBy = maintenanceDate.CompletedBy,
                CreatedAt = maintenanceDate.CreatedAt,
                UpdatedAt = maintenanceDate.UpdatedAt,
                RoomNumber = maintenanceDate.Room?.Number
            };
        }
    }
}
