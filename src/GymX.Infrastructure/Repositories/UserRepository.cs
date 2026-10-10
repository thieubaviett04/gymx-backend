using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Entities.Identity;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

/// <summary>
/// "Thợ chuyên biệt" cho User — kế thừa toàn bộ CRUD từ GenericRepository,
/// chỉ viết thêm các hàm đặc thù mà chỉ bảng User mới cần.
/// </summary>
public class UserRepository(ApplicationDbContext context) : GenericRepository<User>(context), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => Context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => Context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

    public Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
        => Context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);

    public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Code)
            .ToListAsync(cancellationToken);
    }
}
