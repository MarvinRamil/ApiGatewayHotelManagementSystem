using BookManagementSystem.services;
using HotelManagementApplication.Interface.Services;
using HotelManagementInfratructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace BookManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TokenServices _tokenService;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            TokenServices tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterUserAsync(string username, string email, string password, string? role = null)
        {
            var user = new User
            {
                UserName = username,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            role ??= "User";
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            await _userManager.AddToRoleAsync(user, role);

            return (true, Array.Empty<string>());
        }

        public async Task<(bool IsValid, string? AccessToken, string? RefreshToken, string? Error)> LoginUserAsync(string username, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return (false, null, null, "Invalid username or password");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
                if (!result.Succeeded)
                {
                    return (false, null, null, "Invalid username or password");
                }

                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _tokenService.GenerateAccessToken(roles.ToList(), user.UserName!);
                
                // Generate refresh token using Identity's built-in functionality
                var refreshToken = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "RefreshToken");

                return (true, accessToken, refreshToken, null);
            }
            catch (Exception ex)
            {
                return (false, null, null, $"Login error: {ex.Message}");
            }
        }

        public async Task<(bool IsValid, string? AccessToken, string? RefreshToken, string? Error)> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Find user by validating the refresh token
                var user = await _userManager.FindByLoginAsync(TokenOptions.DefaultProvider, refreshToken);
                if (user == null)
                {
                    // If not found by login, try to find by validating the token directly
                    // We need to iterate through users to find the one with valid refresh token
                    var users = _userManager.Users.ToList();
                    User? validUser = null;
                    
                    foreach (var u in users)
                    {
                        if (await _userManager.VerifyUserTokenAsync(u, TokenOptions.DefaultProvider, "RefreshToken", refreshToken))
                        {
                            validUser = u;
                            break;
                        }
                    }
                    
                    if (validUser == null)
                    {
                        return (false, null, null, "Invalid refresh token");
                    }
                    
                    user = validUser;
                }

                // Verify the refresh token is still valid
                var isValidToken = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "RefreshToken", refreshToken);
                if (!isValidToken)
                {
                    return (false, null, null, "Invalid or expired refresh token");
                }

                // Generate new tokens
                var roles = await _userManager.GetRolesAsync(user);
                var newAccessToken = _tokenService.GenerateAccessToken(roles.ToList(), user.UserName!);
                var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "RefreshToken");

                return (true, newAccessToken, newRefreshToken, null);
            }
            catch (Exception ex)
            {
                return (false, null, null, $"Error refreshing token: {ex.Message}");
            }
        }
    }
}
