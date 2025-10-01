using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingServiceApi.Extensions;

namespace BookingServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingOtpController : ControllerBase
    {
        private readonly IBookingOtpService _bookingOtpService;

        public BookingOtpController(IBookingOtpService bookingOtpService)
        {
            _bookingOtpService = bookingOtpService;
        }

        /// <summary>
        /// Generate OTP for booking confirmation
        /// </summary>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateOtp([FromBody] BookingOtpCreateDto request)
        {
            if (!ModelState.IsValid)
            {
                return this.CreateResponse(400, "Invalid OTP generation data", ModelState);
            }

            try
            {
                var result = await _bookingOtpService.GenerateOtpAsync(request.BookingId, request.Email);
                return this.CreateResponse(result.StatusCode, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error generating OTP: {ex.Message}");
            }
        }

        /// <summary>
        /// Verify OTP and confirm booking
        /// </summary>
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] BookingOtpVerifyDto request)
        {
            if (!ModelState.IsValid)
            {
                return this.CreateResponse(400, "Invalid OTP verification data", ModelState);
            }

            try
            {
                var result = await _bookingOtpService.VerifyOtpAsync(request.BookingId, request.OtpCode);
                return this.CreateResponse(result.StatusCode, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error verifying OTP: {ex.Message}");
            }
        }

        /// <summary>
        /// Get OTP details by booking ID
        /// </summary>
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetOtpByBookingId(int bookingId)
        {
            try
            {
                var result = await _bookingOtpService.GetOtpByBookingIdAsync(bookingId);
                return this.CreateResponse(result.StatusCode, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving OTP: {ex.Message}");
            }
        }

        /// <summary>
        /// Resend OTP for booking
        /// </summary>
        [HttpPost("resend/{bookingId}")]
        public async Task<IActionResult> ResendOtp(int bookingId)
        {
            try
            {
                var result = await _bookingOtpService.ResendOtpAsync(bookingId);
                return this.CreateResponse(result.StatusCode, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error resending OTP: {ex.Message}");
            }
        }

        /// <summary>
        /// Expire OTP for booking
        /// </summary>
        [HttpPost("expire/{bookingId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExpireOtp(int bookingId)
        {
            try
            {
                var result = await _bookingOtpService.ExpireOtpAsync(bookingId);
                return this.CreateResponse(result.StatusCode, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error expiring OTP: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all expired OTPs
        /// </summary>
        [HttpGet("expired")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetExpiredOtps()
        {
            try
            {
                var result = await _bookingOtpService.GetExpiredOtpsAsync();
                return this.CreateResponse(result.StatusCode, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error retrieving expired OTPs: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup expired OTPs
        /// </summary>
        [HttpPost("cleanup")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CleanupExpiredOtps()
        {
            try
            {
                var result = await _bookingOtpService.CleanupExpiredOtpsAsync();
                return this.CreateResponse(result.StatusCode, result.Message, result.Data);
            }
            catch (Exception ex)
            {
                return this.CreateResponse(500, $"Error cleaning up expired OTPs: {ex.Message}");
            }
        }
    }
}
