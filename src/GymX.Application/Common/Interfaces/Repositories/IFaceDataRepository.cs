using GymX.Domain.Entities.Identity;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IFaceDataRepository : IGenericRepository<FaceData>
{
    Task<FaceData?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FaceData?> GetByExternalFaceIdAsync(string externalFaceId, CancellationToken cancellationToken = default);
}
