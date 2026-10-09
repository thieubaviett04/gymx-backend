

namespace GymX.Application.Common.Models.Authentication
{
    public record AuthTokenDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public int ExpiresAt { get; init; }
    }
}
