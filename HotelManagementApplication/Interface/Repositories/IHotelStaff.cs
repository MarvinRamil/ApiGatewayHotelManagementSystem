using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using HotelManagementApplication.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Repositories
{
    public interface IHotelStaff : IGeneric<HotelStaff>
    {
        IQueryable<HotelStaff> GetAllStaff();
        Task<HotelStaffDto> createStaff(HotelStaff staff);
        Task<HotelStaff?> GetStaffByEmailAsync(string email);
        Task<List<HotelStaff>> GetActiveStaffAsync();
        Task<List<HotelStaff>> GetStaffByRoleAsync(StaffRoles role);
        Task<List<HotelStaff>> GetStaffByDepartmentAsync(string department);
        Task<List<HotelStaff>> GetStaffBySalaryRangeAsync(decimal minSalary, decimal maxSalary);
        Task<List<HotelStaff>> GetStaffHiredAfterAsync(DateTime hireDate);
    }
}
