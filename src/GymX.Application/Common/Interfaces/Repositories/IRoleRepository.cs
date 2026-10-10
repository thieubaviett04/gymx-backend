using GymX.Domain.Entities.Identity;

namespace GymX.Application.Common.Interfaces.Repositories;

/// <summary>
/// Bản hợp đồng chuyên biệt cho Role — kế thừa CRUD từ IGenericRepository,
/// bổ sung hàm tìm Role theo mã code (ADMIN, MEMBER...).
/// </summary>
public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
