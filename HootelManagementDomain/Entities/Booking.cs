using HootelManagementDomain.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HootelManagementDomain.Entities
{
    public class Booking : BaseEntity
    {
        
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }

        public string BookingNumber { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? SpecialRequests { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        [Required]
        public int GuestId { get; set; }
        [Required]
        public int RoomId { get; set; }
        public Guest Guest { get; set; }
        public Room Room { get; set; }
        public virtual BookingOtp? BookingOtp { get; set; }
    }
}
