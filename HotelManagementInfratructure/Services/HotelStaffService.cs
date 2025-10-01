using HootelManagementDomain.Entities;
using HootelManagementDomain.enums;
using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Repositories;
using HotelManagementApplication.Interface.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementInfratructure.Services
{
    public class HotelStaffService : IHotelStaffService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HotelStaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ICollection<HotelStaffDto>> GetAllStaffAsync()
        {
            var staff = await _unitOfWork.HotelStaff.GetAllAsync();
            return staff.Select(MapToDto).ToList();
        }

        public async Task<HotelStaffDto?> GetStaffByIdAsync(int id)
        {
            var staff = await _unitOfWork.HotelStaff.GetById(id);
            return staff != null ? MapToDto(staff) : null;
        }

        public async Task<HotelStaffDto?> GetStaffByEmailAsync(string email)
        {
            var staff = await _unitOfWork.HotelStaff.GetStaffByEmailAsync(email);
            return staff != null ? MapToDto(staff) : null;
        }

        public async Task<ICollection<HotelStaffDto>> GetActiveStaffAsync()
        {
            var staff = await _unitOfWork.HotelStaff.GetActiveStaffAsync();
            return staff.Select(MapToDto).ToList();
        }

        public async Task<ICollection<HotelStaffDto>> GetStaffByRoleAsync(StaffRoles role)
        {
            var staff = await _unitOfWork.HotelStaff.GetStaffByRoleAsync(role);
            return staff.Select(MapToDto).ToList();
        }

        public async Task<ICollection<HotelStaffDto>> GetStaffByDepartmentAsync(string department)
        {
            var staff = await _unitOfWork.HotelStaff.GetStaffByDepartmentAsync(department);
            return staff.Select(MapToDto).ToList();
        }

        public async Task<HotelStaffDto> CreateStaffAsync(HotelStaffCreateDto staffCreateDto)
        {
            // Check if staff with email already exists
            var existingStaff = await _unitOfWork.HotelStaff.GetStaffByEmailAsync(staffCreateDto.Email);
            if (existingStaff != null)
            {
                throw new InvalidOperationException($"Staff with email {staffCreateDto.Email} already exists.");
            }

            var staff = new HotelStaff
            {
                Name = staffCreateDto.Name,
                Email = staffCreateDto.Email,
                Role = staffCreateDto.Role,
                Phone = staffCreateDto.Phone,
                Salary = staffCreateDto.Salary,
                HireDate = DateTime.UtcNow,
                IsActive = true,
                Department = staffCreateDto.Department,
                EmergencyContact = staffCreateDto.EmergencyContact,
                CreatedBy = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.HotelStaff.Add(staff);
            await _unitOfWork.SaveAsync();

            return MapToDto(staff);
        }

        public async Task<HotelStaffDto?> UpdateStaffAsync(HotelStaffUpdateDto staffUpdateDto)
        {
            var staff = await _unitOfWork.HotelStaff.GetById(staffUpdateDto.Id);
            if (staff == null)
            {
                return null;
            }

            // Check if email already exists (excluding current staff)
            var existingStaff = await _unitOfWork.HotelStaff.GetStaffByEmailAsync(staffUpdateDto.Email);
            if (existingStaff != null && existingStaff.Id != staffUpdateDto.Id)
            {
                throw new InvalidOperationException($"Staff with email {staffUpdateDto.Email} already exists.");
            }

            staff.Name = staffUpdateDto.Name;
            staff.Email = staffUpdateDto.Email;
            staff.Role = staffUpdateDto.Role;
            staff.Phone = staffUpdateDto.Phone;
            staff.Salary = staffUpdateDto.Salary;
            staff.Department = staffUpdateDto.Department;
            staff.EmergencyContact = staffUpdateDto.EmergencyContact;
            staff.IsActive = staffUpdateDto.IsActive;
            staff.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.HotelStaff.Update(staff);
            await _unitOfWork.SaveAsync();

            return MapToDto(staff);
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _unitOfWork.HotelStaff.GetById(id);
            if (staff == null)
            {
                return false;
            }

            // Check if staff has any active responsibilities (this could be extended based on business rules)
            // For now, we'll allow deletion but mark as inactive instead
            staff.IsActive = false;
            staff.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.HotelStaff.Update(staff);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeactivateStaffAsync(int id)
        {
            var staff = await _unitOfWork.HotelStaff.GetById(id);
            if (staff == null)
            {
                return false;
            }

            staff.IsActive = false;
            staff.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.HotelStaff.Update(staff);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> ActivateStaffAsync(int id)
        {
            var staff = await _unitOfWork.HotelStaff.GetById(id);
            if (staff == null)
            {
                return false;
            }

            staff.IsActive = true;
            staff.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.HotelStaff.Update(staff);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<ICollection<HotelStaffDto>> GetStaffBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            var staff = await _unitOfWork.HotelStaff.GetStaffBySalaryRangeAsync(minSalary, maxSalary);
            return staff.Select(MapToDto).ToList();
        }

        public async Task<ICollection<HotelStaffDto>> GetStaffHiredAfterAsync(DateTime hireDate)
        {
            var staff = await _unitOfWork.HotelStaff.GetStaffHiredAfterAsync(hireDate);
            return staff.Select(MapToDto).ToList();
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
