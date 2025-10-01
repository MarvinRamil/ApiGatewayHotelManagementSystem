using HotelManagementApplication.Dto;
using HotelManagementApplication.Interface.Services;
using Microsoft.AspNetCore.Mvc;
using BookingServiceApi.Extensions;

namespace BookManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (succeeded, errors) = await _authService.RegisterUserAsync(request.Username, request.Email, request.Password, request.Role);

            if (!succeeded)
                return BadRequest(new { Errors = errors });

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.CreateResponse(400, "Invalid login data", ModelState);
            }

            var (isValid, accessToken, refreshToken, error) = await _authService.LoginUserAsync(request.Username, request.Password);

            if (!isValid)
                return this.CreateResponse(401, error ?? "Login failed");

            var response = new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 900 // 15 minutes in seconds
            };

            return this.CreateResponse(200, "Login successful", response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.CreateResponse(400, "Invalid refresh token data", ModelState);
            }

            var (isValid, accessToken, refreshToken, error) = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (!isValid)
                return this.CreateResponse(401, error ?? "Token refresh failed");

            var response = new RefreshTokenResponse
            {
                AccessToken = accessToken!,
                RefreshToken = refreshToken!,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                TokenType = "Bearer"
            };

            return this.CreateResponse(200, "Token refreshed successfully", response);
        }
    }
}
