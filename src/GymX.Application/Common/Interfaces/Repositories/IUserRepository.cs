using GymX.Domain.Entities.Identity;

namespace GymX.Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
