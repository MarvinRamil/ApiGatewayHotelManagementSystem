using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementApplication.Interface.Services;

namespace HotelManagementInfratructure.Services
{
    public class MaintenanceDateService : IMaintenanceDateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMaintenanceDate _maintenanceDateRepository;
        private readonly IRoom _roomRepository;

        public MaintenanceDateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _maintenanceDateRepository = unitOfWork.MaintenanceDate;
            _roomRepository = unitOfWork.Room;
        }

        public async Task<ApiResponse<MaintenanceDateDto>> CreateMaintenanceDateAsync(MaintenanceDateCreateDto createDto)
        {
            try
            {
                // Validate room exists
                var room = await _roomRepository.GetById(createDto.RoomId);
                if (room == null)
                {
                    return ResponseHelper.Failure<MaintenanceDateDto>("Room not found", 404);
                }

                // Check for maintenance conflicts
                var hasConflict = await _maintenanceDateRepository.HasMaintenanceConflictAsync(createDto.RoomId, createDto.Date);
                if (hasConflict)
                {
                    return ResponseHelper.Failure<MaintenanceDateDto>("Maintenance already scheduled for this room on this date", 400);
                }

                var maintenanceDate = new MaintenanceDate
                {
                    Date = createDto.Date,
                    Status = createDto.Status,
                    RoomId = createDto.RoomId,
                    Description = createDto.Description,
                    Notes = createDto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _maintenanceDateRepository.Add(maintenanceDate);
                await _unitOfWork.Save();

                var result = await _maintenanceDateRepository.GetMaintenanceDateByIdAsync(maintenanceDate.Id);
                return ResponseHelper.Success(result!, "Maintenance date created successfully", 201);
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<MaintenanceDateDto>($"Error creating maintenance date: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<MaintenanceDateDto>> GetMaintenanceDateByIdAsync(int id)
        {
            try
            {
                var maintenanceDate = await _maintenanceDateRepository.GetMaintenanceDateByIdAsync(id);
                if (maintenanceDate == null)
                {
                    return ResponseHelper.Failure<MaintenanceDateDto>("Maintenance date not found", 404);
                }

                return ResponseHelper.Success(maintenanceDate, "Maintenance date retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<MaintenanceDateDto>($"Error retrieving maintenance date: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetAllMaintenanceDatesAsync()
        {
            try
            {
                var maintenanceDates = await _maintenanceDateRepository.GetAllAsync();
                var maintenanceDateDtos = maintenanceDates.Select(MapToDto).ToList();

                return ResponseHelper.Success<IEnumerable<MaintenanceDateDto>>(maintenanceDateDtos, "Maintenance dates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<IEnumerable<MaintenanceDateDto>>($"Error retrieving maintenance dates: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<MaintenanceDateDto>> UpdateMaintenanceDateAsync(MaintenanceDateUpdateDto updateDto)
        {
            try
            {
                var existingMaintenanceDate = await _maintenanceDateRepository.GetById(updateDto.Id);
                if (existingMaintenanceDate == null)
                {
                    return ResponseHelper.Failure<MaintenanceDateDto>("Maintenance date not found", 404);
                }

                // Validate room exists
                var room = await _roomRepository.GetById(updateDto.RoomId);
                if (room == null)
                {
                    return ResponseHelper.Failure<MaintenanceDateDto>("Room not found", 404);
                }

                // Check for maintenance conflicts (excluding current record)
                var hasConflict = await _maintenanceDateRepository.HasMaintenanceConflictAsync(updateDto.RoomId, updateDto.Date, updateDto.Id);
                if (hasConflict)
                {
                    return ResponseHelper.Failure<MaintenanceDateDto>("Maintenance already scheduled for this room on this date", 400);
                }

                existingMaintenanceDate.Date = updateDto.Date;
                existingMaintenanceDate.Status = updateDto.Status;
                existingMaintenanceDate.RoomId = updateDto.RoomId;
                existingMaintenanceDate.Description = updateDto.Description;
                existingMaintenanceDate.Notes = updateDto.Notes;
                existingMaintenanceDate.CompletedAt = updateDto.CompletedAt;
                existingMaintenanceDate.CompletedBy = updateDto.CompletedBy;
                existingMaintenanceDate.UpdatedAt = DateTime.UtcNow;

                _maintenanceDateRepository.Update(existingMaintenanceDate);
                await _unitOfWork.Save();

                var result = await _maintenanceDateRepository.GetMaintenanceDateByIdAsync(existingMaintenanceDate.Id);
                return ResponseHelper.Success(result!, "Maintenance date updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<MaintenanceDateDto>($"Error updating maintenance date: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteMaintenanceDateAsync(int id)
        {
            try
            {
                var maintenanceDate = await _maintenanceDateRepository.GetById(id);
                if (maintenanceDate == null)
                {
                    return ResponseHelper.Failure<bool>("Maintenance date not found", 404);
                }

                _maintenanceDateRepository.Delete(maintenanceDate);
                await _unitOfWork.Save();

                return ResponseHelper.Success(true, "Maintenance date deleted successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<bool>($"Error deleting maintenance date: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<MaintenanceDateDto>> UpdateMaintenanceDateStatusAsync(int id, MaintenanceDateStatusUpdateDto statusUpdateDto)
        {
            try
            {
                var maintenanceDate = await _maintenanceDateRepository.GetById(id);
                if (maintenanceDate == null)
                {
                    return ResponseHelper.Failure<MaintenanceDateDto>("Maintenance date not found", 404);
                }

                maintenanceDate.Status = statusUpdateDto.Status;
                maintenanceDate.Notes = statusUpdateDto.Notes;
                maintenanceDate.UpdatedAt = DateTime.UtcNow;

                if (statusUpdateDto.Status == MaintenanceStatus.Completed)
                {
                    maintenanceDate.CompletedAt = DateTime.UtcNow;
                    maintenanceDate.CompletedBy = statusUpdateDto.CompletedBy;
                }

                _maintenanceDateRepository.Update(maintenanceDate);
                await _unitOfWork.Save();

                var result = await _maintenanceDateRepository.GetMaintenanceDateByIdAsync(maintenanceDate.Id);
                return ResponseHelper.Success(result!, "Maintenance date status updated successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<MaintenanceDateDto>($"Error updating maintenance date status: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesByRoomIdAsync(int roomId)
        {
            try
            {
                var maintenanceDates = await _maintenanceDateRepository.GetMaintenanceDatesByRoomIdAsync(roomId);
                return ResponseHelper.Success<IEnumerable<MaintenanceDateDto>>(maintenanceDates, "Maintenance dates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<IEnumerable<MaintenanceDateDto>>($"Error retrieving maintenance dates: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesByStatusAsync(MaintenanceStatus status)
        {
            try
            {
                var maintenanceDates = await _maintenanceDateRepository.GetMaintenanceDatesByStatusAsync(status);
                return ResponseHelper.Success<IEnumerable<MaintenanceDateDto>>(maintenanceDates, "Maintenance dates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<IEnumerable<MaintenanceDateDto>>($"Error retrieving maintenance dates: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var maintenanceDates = await _maintenanceDateRepository.GetMaintenanceDatesByDateRangeAsync(startDate, endDate);
                return ResponseHelper.Success<IEnumerable<MaintenanceDateDto>>(maintenanceDates, "Maintenance dates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<IEnumerable<MaintenanceDateDto>>($"Error retrieving maintenance dates: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetUpcomingMaintenanceDatesAsync()
        {
            try
            {
                var maintenanceDates = await _maintenanceDateRepository.GetUpcomingMaintenanceDatesAsync();
                return ResponseHelper.Success<IEnumerable<MaintenanceDateDto>>(maintenanceDates, "Upcoming maintenance dates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<IEnumerable<MaintenanceDateDto>>($"Error retrieving upcoming maintenance dates: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesForDateAsync(DateTime date)
        {
            try
            {
                var maintenanceDates = await _maintenanceDateRepository.GetMaintenanceDatesForDateAsync(date);
                return ResponseHelper.Success<IEnumerable<MaintenanceDateDto>>(maintenanceDates, "Maintenance dates for date retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<IEnumerable<MaintenanceDateDto>>($"Error retrieving maintenance dates for date: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetOverdueMaintenanceDatesAsync()
        {
            try
            {
                var maintenanceDates = await _maintenanceDateRepository.GetOverdueMaintenanceDatesAsync();
                return ResponseHelper.Success<IEnumerable<MaintenanceDateDto>>(maintenanceDates, "Overdue maintenance dates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<IEnumerable<MaintenanceDateDto>>($"Error retrieving overdue maintenance dates: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<MaintenanceDateDto>>> GetMaintenanceDatesByRoomAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var maintenanceDates = await _maintenanceDateRepository.GetMaintenanceDatesByRoomAndDateRangeAsync(roomId, startDate, endDate);
                return ResponseHelper.Success<IEnumerable<MaintenanceDateDto>>(maintenanceDates, "Maintenance dates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<IEnumerable<MaintenanceDateDto>>($"Error retrieving maintenance dates: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> CheckMaintenanceConflictAsync(int roomId, DateTime date, int? excludeId = null)
        {
            try
            {
                var hasConflict = await _maintenanceDateRepository.HasMaintenanceConflictAsync(roomId, date, excludeId);
                return ResponseHelper.Success(hasConflict, "Conflict check completed");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<bool>($"Error checking maintenance conflict: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> CompleteMaintenanceAsync(int id, string completedBy)
        {
            try
            {
                var maintenanceDate = await _maintenanceDateRepository.GetById(id);
                if (maintenanceDate == null)
                {
                    return ResponseHelper.Failure<bool>("Maintenance date not found", 404);
                }

                maintenanceDate.Status = MaintenanceStatus.Completed;
                maintenanceDate.CompletedAt = DateTime.UtcNow;
                maintenanceDate.CompletedBy = completedBy;
                maintenanceDate.UpdatedAt = DateTime.UtcNow;

                _maintenanceDateRepository.Update(maintenanceDate);
                await _unitOfWork.Save();

                return ResponseHelper.Success(true, "Maintenance marked as completed successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<bool>($"Error completing maintenance: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> CancelMaintenanceAsync(int id, string reason)
        {
            try
            {
                var maintenanceDate = await _maintenanceDateRepository.GetById(id);
                if (maintenanceDate == null)
                {
                    return ResponseHelper.Failure<bool>("Maintenance date not found", 404);
                }

                maintenanceDate.Status = MaintenanceStatus.Cancelled;
                maintenanceDate.Notes = reason;
                maintenanceDate.UpdatedAt = DateTime.UtcNow;

                _maintenanceDateRepository.Update(maintenanceDate);
                await _unitOfWork.Save();

                return ResponseHelper.Success(true, "Maintenance cancelled successfully");
            }
            catch (Exception ex)
            {
                return ResponseHelper.Failure<bool>($"Error cancelling maintenance: {ex.Message}", 500);
            }
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
                UpdatedAt = maintenanceDate.UpdatedAt
            };
        }
    }
}