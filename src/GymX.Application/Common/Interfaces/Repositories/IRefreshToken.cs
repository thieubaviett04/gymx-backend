using GymX.Domain.Entities.Identity;


namespace GymX.Application.Common.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        public Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
