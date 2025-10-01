using HotelManagementApplication.Dto;

namespace HotelManagementApplication.Interface.Services
{
    public interface IEmailService
    {
        Task<EmailResponseDto> SendEmailAsync(EmailDto emailDto);
        Task<EmailResponseDto> SendTemplateEmailAsync(EmailTemplateDto templateDto);
        Task<EmailResponseDto> SendBookingConfirmationAsync(BookingEmailData bookingData);
        Task<EmailResponseDto> SendBookingCancellationAsync(BookingEmailData bookingData);
        Task<EmailResponseDto> SendBookingReminderAsync(BookingEmailData bookingData);
        Task<EmailResponseDto> SendCheckInReminderAsync(BookingEmailData bookingData);
        Task<EmailResponseDto> SendCheckOutReminderAsync(BookingEmailData bookingData);
        Task<EmailResponseDto> SendMaintenanceNotificationAsync(MaintenanceEmailData maintenanceData);
        Task<EmailResponseDto> SendWelcomeEmailAsync(string to, string userName);
        Task<EmailResponseDto> SendPasswordResetEmailAsync(PasswordResetEmailData resetData);
        Task<EmailResponseDto> SendInvoiceEmailAsync(BookingEmailData bookingData, byte[] invoicePdf);
        Task<bool> ValidateEmailConfigurationAsync();
        Task<List<EmailResponseDto>> SendBulkEmailsAsync(List<EmailDto> emailDtos);
    }
}
