using GymX.Domain.Entities.Members;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<Member?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
