using HootelManagementDomain.enums;
using System;

namespace HootelManagementDomain.Entities
{
    public class BookingOtp : BaseEntity
    {
        public int BookingId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
        public OtpStatus Status { get; set; } = OtpStatus.Pending;

        // Navigation property
        public virtual Booking Booking { get; set; } = null!;
    }
}
