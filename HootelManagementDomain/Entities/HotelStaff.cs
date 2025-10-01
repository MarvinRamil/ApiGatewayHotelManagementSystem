using HootelManagementDomain.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HootelManagementDomain.Entities
{
    public class HotelStaff : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public StaffRoles Role { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public string? Department { get; set; }
        public string? EmergencyContact { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
