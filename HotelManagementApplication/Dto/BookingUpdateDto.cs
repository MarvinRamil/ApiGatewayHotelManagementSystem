using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class BookingUpdateDto
    {
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
        [Range(1, 10)]
        public int Guests { get; set; }
        public string? SpecialRequests { get; set; }
    }
}
