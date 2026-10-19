namespace GymX.Application.Common.Models.Authentication
{
    public record AuthTokenDto
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public int ExpiresAt { get; init; }
        public IList<string> Roles { get; init; } = [];
        public Guid UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string? AvatarUrl { get; init; }
    }
}
