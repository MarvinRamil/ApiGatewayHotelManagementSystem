using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class RoomImageUpdateDto
    {
        [Required]
        public int RoomId { get; set; }
        
        [Required]
        public List<string> ImageUrls { get; set; } = new List<string>();
        
        public string? PrimaryImageUrl { get; set; }
    }
}

