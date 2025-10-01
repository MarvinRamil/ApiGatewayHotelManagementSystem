using HootelManagementDomain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public RoomStatus Status { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public List<string> Amenities { get; set; } = new List<string>();
        public int Floor { get; set; }
        public DateTime? LastCleaned { get; set; }
        public string? CurrentGuest { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
