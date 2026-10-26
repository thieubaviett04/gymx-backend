using GymX.Domain.Entities.Identity;

namespace GymX.Application.Common.Interfaces.Authencation
{
    public record IJwtServiceResult
    (
        string AccessToken,
        string RefreshToken,
        Guid SessionId,
        int AccessTokenExpiresIn,
        DateTime AccessTokenExpiresAt,
        DateTime RefreshTokenExpiresAt
    );

    public interface IJwtService
    {
        IJwtServiceResult GenerateTokens(User user, IList<string> roles);
    }
}
