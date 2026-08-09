using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Entities.Identity;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        
        public Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
            => context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await context.Users.AddAsync(user, cancellationToken);
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Code)
                .ToListAsync(cancellationToken);
        }
    }
}
