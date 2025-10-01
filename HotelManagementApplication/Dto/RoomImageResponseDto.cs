using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class RoomImageResponseDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public bool IsPrimary { get; set; }
    }
}

