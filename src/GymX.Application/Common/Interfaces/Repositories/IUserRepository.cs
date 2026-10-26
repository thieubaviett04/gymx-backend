using GymX.Domain.Entities.Identity;

namespace GymX.Application.Common.Interfaces.Repositories;

/// <summary>
/// Bản hợp đồng chuyên biệt cho User — kế thừa toàn bộ CRUD từ IGenericRepository,
/// chỉ bổ sung thêm các hàm riêng mà User mới cần (tìm theo Email, GoogleId...).
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}
