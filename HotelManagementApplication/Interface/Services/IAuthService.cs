namespace HotelManagementApplication.Interface.Services
{
    public interface IAuthService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterUserAsync(string username, string email, string password, string? role = null);
        Task<(bool IsValid, string? AccessToken, string? RefreshToken, string? Error)> LoginUserAsync(string username, string password);
        Task<(bool IsValid, string? AccessToken, string? RefreshToken, string? Error)> RefreshTokenAsync(string refreshToken);
    }
}
