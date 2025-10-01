using HootelManagementDomain.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Dto
{
    public class BookingStatusUpdateDto
    {
        [Required]
        public BookingStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
