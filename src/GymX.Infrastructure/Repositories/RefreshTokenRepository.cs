using GymX.Application.Common.Interfaces.Repositories;
using GymX.Infrastructure.Persistence;
using GymX.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories
{
    public class RefreshTokenRepository(ApplicationDbContext context) : GenericRepository<RefreshToken>(context), IRefreshTokenRepository
    {
        public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return Context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.TokenHash == token, cancellationToken);
        }

        public async Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var refreshTokens = await Context.RefreshTokens
                .Where(r => r.UserId == userId && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var refreshToken in refreshTokens)
            {
                refreshToken.Revoke();
            }
        }
    }
}
