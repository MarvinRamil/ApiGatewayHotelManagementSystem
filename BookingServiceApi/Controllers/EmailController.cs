using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingServiceApi.Extensions;

namespace BookingServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Test email with simple message
        /// </summary>
        [HttpPost("test")]
        [Authorize]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.CreateResponse(400, "Invalid test email data", ModelState);
            }

            try
            {
                var emailDto = new EmailDto
                {
                    To = request.To,
                    Subject = "Test Email",
                    Body = "Email works",
                    IsHtml = false
                };

                var result = await _emailService.SendEmailAsync(emailDto);
                return this.CreateResponse(result.Success ? 200 : 400, result.Message, result);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error sending test email: {ex.Message}");
            }
        }
    }

    public class TestEmailRequest
    {
        public string To { get; set; } = string.Empty;
    }
}