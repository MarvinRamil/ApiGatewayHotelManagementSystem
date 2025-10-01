using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class RoomUpdateDto
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string Number { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10")]
        public int Capacity { get; set; }
        
        public List<string> Amenities { get; set; } = new List<string>();
        
        [Required]
        [Range(1, 50, ErrorMessage = "Floor must be between 1 and 50")]
        public int Floor { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public List<string> Images { get; set; } = new List<string>();
    }
}
