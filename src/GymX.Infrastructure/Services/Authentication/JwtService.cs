
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Domain.Entities.Identity;
using GymX.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GymX.Infrastructure.Services.Authentication
{
    public class JwtService(IOptions<JwtOptions> option) : IJwtService
    {
        private readonly JwtOptions _options = option.Value;

        public IJwtServiceResult GenerateTokens(User user, IList<string> roles)
        {
            var sessionId = Guid.NewGuid();

            // Tao Claims cho JWT
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, sessionId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullName", user.FullName),
            };

            // Gan Role vao Claims de phan quyen
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Lay key va credentials de ky token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);

            // Tao access token
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: accessTokenExpiresAt,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var refreshToken = Convert.ToBase64String(randomNumber);
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenExpiryDays);

            return new IJwtServiceResult
            (
                AccessToken : accessToken,
                RefreshToken : refreshToken,
                SessionId: sessionId,
                AccessTokenExpiresIn : _options.ExpiryMinutes * 60, // Convert minutes to seconds
                AccessTokenExpiresAt : accessTokenExpiresAt,
                RefreshTokenExpiresAt : refreshTokenExpiresAt
            );
        }
    }
}
