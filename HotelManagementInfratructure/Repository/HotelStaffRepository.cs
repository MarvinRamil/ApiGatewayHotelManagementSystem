using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementInfratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementInfratructure.Repository
{
    public class HotelStaffRepository : GenericRepository<HotelStaff>, IHotelStaff
    {
        public HotelStaffRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<HotelStaff> GetAllStaff()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }

        public async Task<HotelStaffDto> createStaff(HotelStaff staff)
        {
            await Add(staff);
            await _context.SaveChangesAsync();
            
            return MapToDto(staff);
        }

        public async Task<HotelStaff?> GetStaffByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<List<HotelStaff>> GetActiveStaffAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<List<HotelStaff>> GetStaffByRoleAsync(StaffRoles role)
        {
            return await _dbSet
                .Where(s => s.Role == role)
                .ToListAsync();
        }

        public async Task<List<HotelStaff>> GetStaffByDepartmentAsync(string department)
        {
            return await _dbSet
                .Where(s => s.Department == department)
                .ToListAsync();
        }

        public async Task<List<HotelStaff>> GetStaffBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            return await _dbSet
                .Where(s => s.Salary >= minSalary && s.Salary <= maxSalary)
                .ToListAsync();
        }

        public async Task<List<HotelStaff>> GetStaffHiredAfterAsync(DateTime hireDate)
        {
            return await _dbSet
                .Where(s => s.HireDate >= hireDate)
                .ToListAsync();
        }

        private static HotelStaffDto MapToDto(HotelStaff staff)
        {
            return new HotelStaffDto
            {
                Id = staff.Id,
                Name = staff.Name,
                Email = staff.Email,
                Role = staff.Role,
                Phone = staff.Phone,
                Salary = staff.Salary,
                HireDate = staff.HireDate,
                IsActive = staff.IsActive,
                Department = staff.Department,
                EmergencyContact = staff.EmergencyContact,
                CreatedAt = staff.CreatedBy,
                UpdatedAt = staff.UpdatedAt
            };
        }
    }
}
