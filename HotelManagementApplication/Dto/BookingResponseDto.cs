using HootelManagementDomain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class BookingResponseDto
    {
        public required int Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public RoomDto? Room { get; set; }
        public GuestDto? Guest { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public BookingStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
