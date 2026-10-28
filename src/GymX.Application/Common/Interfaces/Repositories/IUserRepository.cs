using GymX.Application.Modules.Users.DTOs;
using GymX.Domain.Entities.Identity;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> GetFullProfileAsync(Guid userId, CancellationToken cancellationToken = default);
}
