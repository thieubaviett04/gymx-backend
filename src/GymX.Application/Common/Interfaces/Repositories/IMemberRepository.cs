using GymX.Application.Modules.Members.DTOs.Response;
using GymX.Domain.Entities.Members;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<Member?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<MemberProfileDto?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
