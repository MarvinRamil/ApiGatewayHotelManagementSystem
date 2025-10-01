using HotelManagementApplication.Dto;
using HootelManagementDomain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Services
{
    public interface IHotelStaffService
    {
        Task<ICollection<HotelStaffDto>> GetAllStaffAsync();
        Task<HotelStaffDto?> GetStaffByIdAsync(int id);
        Task<HotelStaffDto?> GetStaffByEmailAsync(string email);
        Task<ICollection<HotelStaffDto>> GetActiveStaffAsync();
        Task<ICollection<HotelStaffDto>> GetStaffByRoleAsync(StaffRoles role);
        Task<ICollection<HotelStaffDto>> GetStaffByDepartmentAsync(string department);
        Task<HotelStaffDto> CreateStaffAsync(HotelStaffCreateDto staffCreateDto);
        Task<HotelStaffDto?> UpdateStaffAsync(HotelStaffUpdateDto staffUpdateDto);
        Task<bool> DeleteStaffAsync(int id);
        Task<bool> DeactivateStaffAsync(int id);
        Task<bool> ActivateStaffAsync(int id);
        Task<ICollection<HotelStaffDto>> GetStaffBySalaryRangeAsync(decimal minSalary, decimal maxSalary);
        Task<ICollection<HotelStaffDto>> GetStaffHiredAfterAsync(DateTime hireDate);
    }
}
