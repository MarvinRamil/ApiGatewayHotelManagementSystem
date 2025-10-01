# 📧 Email Service Documentation

## Overview
A comprehensive email service built with **MailKit** for the Hotel Management System. This service provides email functionality for booking confirmations, maintenance notifications, password resets, and more.

## 🚀 Features

### ✅ **Core Email Functionality**
- **Custom Emails**: Send any custom email with HTML/text content
- **Template-Based Emails**: Pre-built templates for common hotel operations
- **Attachment Support**: Send emails with file attachments
- **Bulk Email**: Send multiple emails efficiently
- **Email Validation**: Validate SMTP configuration

### ✅ **Hotel-Specific Templates**
- **Booking Confirmation**: Welcome guests with booking details
- **Booking Cancellation**: Notify guests of cancellations
- **Booking Reminders**: Remind guests of upcoming stays
- **Check-in/Check-out Reminders**: Automated guest notifications
- **Maintenance Notifications**: Alert staff of maintenance schedules
- **Welcome Emails**: New user onboarding
- **Password Reset**: Secure password recovery
- **Invoice Emails**: Send invoices with PDF attachments

## 📋 Configuration

### 1. **Update appsettings.json**
```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "UseSsl": false,
    "FromEmail": "noreply@hotel.com",
    "FromName": "Hotel Management System",
    "AdminEmail": "admin@hotel.com",
    "HotelName": "Hotel Management System",
    "HotelAddress": "123 Hotel Street, City, Country",
    "HotelPhone": "+1-234-567-8900"
  }
}
```

### 2. **Gmail Setup (Recommended)**
1. Enable 2-Factor Authentication on your Gmail account
2. Generate an App Password:
   - Go to Google Account settings
   - Security → 2-Step Verification → App passwords
   - Generate password for "Mail"
3. Use the app password in `SmtpPassword`

### 3. **Other SMTP Providers**
- **Outlook**: `smtp-mail.outlook.com:587`
- **Yahoo**: `smtp.mail.yahoo.com:587`
- **Custom SMTP**: Configure your own SMTP server

## 🔧 API Endpoints

### **Base URL**: `/api/Email`

### 1. **Send Custom Email**
```http
POST /api/Email/send
Content-Type: application/json

{
  "to": "guest@example.com",
  "subject": "Welcome to Our Hotel",
  "body": "<h1>Welcome!</h1><p>Thank you for choosing our hotel.</p>",
  "isHtml": true,
  "attachments": [
    {
      "fileName": "welcome.pdf",
      "content": "base64-encoded-content",
      "contentType": "application/pdf"
    }
  ]
}
```

### 2. **Send Booking Confirmation**
```http
POST /api/Email/booking/confirmation
Content-Type: application/json

{
  "guestName": "John Doe",
  "guestEmail": "john@example.com",
  "bookingNumber": "BK001",
  "roomNumber": "101",
  "roomType": "Deluxe Suite",
  "checkIn": "2024-01-15",
  "checkOut": "2024-01-18",
  "totalAmount": 450.00,
  "hotelName": "Grand Hotel",
  "hotelAddress": "123 Main St, City",
  "hotelPhone": "+1-234-567-8900",
  "specialRequests": "Late check-in requested"
}
```

### 3. **Send Maintenance Notification**
```http
POST /api/Email/maintenance/notification
Content-Type: application/json

{
  "roomNumber": "101",
  "maintenanceType": "Plumbing",
  "scheduledDate": "2024-01-15",
  "description": "Fix leaky faucet in bathroom",
  "notes": "Guest reported issue",
  "hotelName": "Grand Hotel"
}
```

### 4. **Send Password Reset**
```http
POST /api/Email/password-reset
Content-Type: application/json

{
  "userName": "user@example.com",
  "resetToken": "abc123def456",
  "resetUrl": "https://hotel.com/reset-password?token=abc123def456",
  "expiryTime": "2024-01-15T23:59:59"
}
```

### 5. **Send Bulk Emails**
```http
POST /api/Email/bulk
Content-Type: application/json

[
  {
    "to": "guest1@example.com",
    "subject": "Newsletter",
    "body": "Monthly hotel newsletter"
  },
  {
    "to": "guest2@example.com",
    "subject": "Newsletter",
    "body": "Monthly hotel newsletter"
  }
]
```

### 6. **Validate Email Configuration**
```http
GET /api/Email/validate-config
```

## 💻 Usage Examples

### **C# Service Integration**
```csharp
public class BookingService
{
    private readonly IEmailService _emailService;
    
    public async Task<BookingResponseDto> CreateBookingAsync(BookingCreateDto bookingDto)
    {
        // Create booking logic...
        var booking = await CreateBooking(bookingDto);
        
        // Send confirmation email
        var emailData = new BookingEmailData
        {
            GuestName = booking.Guest.FirstName + " " + booking.Guest.LastName,
            GuestEmail = booking.Guest.Email,
            BookingNumber = booking.BookingNumber,
            RoomNumber = booking.Room.Number,
            RoomType = booking.Room.Type,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            TotalAmount = booking.TotalAmount,
            HotelName = "Grand Hotel"
        };
        
        await _emailService.SendBookingConfirmationAsync(emailData);
        
        return booking;
    }
}
```

### **Maintenance Integration**
```csharp
public class MaintenanceDateService
{
    private readonly IEmailService _emailService;
    
    public async Task<MaintenanceDateDto> CreateMaintenanceDateAsync(MaintenanceDateCreateDto createDto)
    {
        // Create maintenance logic...
        var maintenance = await CreateMaintenance(createDto);
        
        // Send notification email
        var emailData = new MaintenanceEmailData
        {
            RoomNumber = maintenance.Room.Number,
            MaintenanceType = "Scheduled Maintenance",
            ScheduledDate = maintenance.Date,
            Description = maintenance.Description,
            HotelName = "Grand Hotel"
        };
        
        await _emailService.SendMaintenanceNotificationAsync(emailData);
        
        return maintenance;
    }
}
```

## 🎨 Email Templates

### **Booking Confirmation Template**
```html
<html>
<body style="font-family: Arial, sans-serif;">
    <h2>Booking Confirmation</h2>
    <p>Dear {{GuestName}},</p>
    <p>Your booking has been confirmed. Here are the details:</p>
    <ul>
        <li><strong>Booking Number:</strong> {{BookingNumber}}</li>
        <li><strong>Room:</strong> {{RoomNumber}} - {{RoomType}}</li>
        <li><strong>Check-in:</strong> {{CheckIn}}</li>
        <li><strong>Check-out:</strong> {{CheckOut}}</li>
        <li><strong>Total Amount:</strong> ${{TotalAmount}}</li>
    </ul>
    <p>We look forward to welcoming you!</p>
    <p>Best regards,<br>{{HotelName}}</p>
</body>
</html>
```

## 🔒 Security Features

- **SMTP Authentication**: Secure email sending with credentials
- **SSL/TLS Support**: Encrypted email transmission
- **Input Validation**: Email address and content validation
- **Error Handling**: Comprehensive error logging and handling
- **Rate Limiting**: Built-in delays for bulk email operations

## 📊 Response Format

### **Success Response**
```json
{
  "success": true,
  "message": "Email sent successfully",
  "data": {
    "success": true,
    "message": "Email sent successfully",
    "sentAt": "2024-01-15T10:30:00Z"
  },
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### **Error Response**
```json
{
  "success": false,
  "message": "Failed to send email",
  "data": {
    "success": false,
    "message": "Failed to send email",
    "errorDetails": "SMTP authentication failed",
    "sentAt": "2024-01-15T10:30:00Z"
  },
  "statusCode": 400,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 🚨 Troubleshooting

### **Common Issues**

1. **SMTP Authentication Failed**
   - Check username/password
   - Ensure 2FA is enabled and app password is used
   - Verify SMTP settings

2. **Connection Timeout**
   - Check firewall settings
   - Verify SMTP host and port
   - Try different SSL/TLS settings

3. **Email Not Delivered**
   - Check spam folder
   - Verify recipient email address
   - Check SMTP server logs

### **Testing Email Configuration**
```http
GET /api/Email/validate-config
```
This endpoint tests your SMTP configuration without sending actual emails.

## 📈 Performance Tips

1. **Bulk Operations**: Use bulk email endpoint for multiple recipients
2. **Async Operations**: All email operations are asynchronous
3. **Connection Pooling**: MailKit handles connection pooling automatically
4. **Error Handling**: Implement retry logic for failed emails
5. **Rate Limiting**: Built-in delays prevent SMTP server overload

## 🔄 Integration with Hotel System

The email service integrates seamlessly with:
- **Booking System**: Automatic confirmation emails
- **Maintenance System**: Maintenance notifications
- **User Management**: Welcome emails and password resets
- **Invoice System**: Invoice delivery with attachments

## 📝 Next Steps

1. **Configure SMTP Settings**: Update appsettings.json with your email provider
2. **Test Configuration**: Use the validation endpoint
3. **Integrate with Services**: Add email calls to your business logic
4. **Customize Templates**: Modify email templates for your brand
5. **Monitor Performance**: Set up logging and monitoring

---

**🎉 Your email service is ready to use!** Start by configuring your SMTP settings and testing with the validation endpoint.
