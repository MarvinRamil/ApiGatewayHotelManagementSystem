using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class RoomImageUploadDto
    {
        [Required]
        public int RoomId { get; set; }
        
        [Required]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        
        public string? Description { get; set; }
    }
}
