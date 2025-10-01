namespace HotelManagementApplication.Dto
{
    public class EmailTemplateDto
    {
        public string To { get; set; } = string.Empty;
        public string? Cc { get; set; }
        public string? Bcc { get; set; }
        public EmailTemplateType TemplateType { get; set; }
        public Dictionary<string, object> TemplateData { get; set; } = new Dictionary<string, object>();
        public List<EmailAttachmentDto>? Attachments { get; set; }
    }

    public enum EmailTemplateType
    {
        BookingConfirmation = 1,
        BookingCancellation = 2,
        BookingReminder = 3,
        CheckInReminder = 4,
        CheckOutReminder = 5,
        MaintenanceNotification = 6,
        WelcomeEmail = 7,
        PasswordReset = 8,
        Invoice = 9,
        Custom = 10
    }

    public class BookingEmailData
    {
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string BookingNumber { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalAmount { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string HotelAddress { get; set; } = string.Empty;
        public string HotelPhone { get; set; } = string.Empty;
        public string? SpecialRequests { get; set; }
    }

    public class MaintenanceEmailData
    {
        public string RoomNumber { get; set; } = string.Empty;
        public string MaintenanceType { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string HotelName { get; set; } = string.Empty;
    }

    public class PasswordResetEmailData
    {
        public string UserName { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public string ResetUrl { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
    }
}
