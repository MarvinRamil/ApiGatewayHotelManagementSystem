using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class GuestUpdateDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? IdNumber { get; set; }
        
        [StringLength(50)]
        public string? Nationality { get; set; }
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        public List<string> Preferences { get; set; } = new List<string>();
        
        public bool IsActive { get; set; }
    }
}
