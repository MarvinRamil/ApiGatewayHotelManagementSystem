using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class GuestDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? IdNumber { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public List<string> Preferences { get; set; } = new List<string>();
        public int TotalStays { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
