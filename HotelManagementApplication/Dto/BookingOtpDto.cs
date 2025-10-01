using HootelManagementDomain.enums;

namespace HotelManagementApplication.Dto
{
    public class BookingOtpDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedAt { get; set; }
        public OtpStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BookingOtpCreateDto
    {
        public int BookingId { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public class BookingOtpVerifyDto
    {
        public int BookingId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
    }

    public class BookingOtpResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int BookingId { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}
