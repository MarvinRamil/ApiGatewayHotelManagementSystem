using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class BookingSearchDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? GuestId { get; set; }
        public int? RoomId { get; set; }
        public string? BookingNumber { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestName { get; set; }
    }
}
