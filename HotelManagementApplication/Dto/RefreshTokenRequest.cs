using System.ComponentModel.DataAnnotations;

namespace HotelManagementApplication.Dto
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
