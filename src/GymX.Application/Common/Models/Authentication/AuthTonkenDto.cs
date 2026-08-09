

namespace GymX.Application.Common.Models.Authentication
{
    public class AuthTokenDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresAt { get; set; }
    }
}
