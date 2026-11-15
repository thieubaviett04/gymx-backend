using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Entities.Identity;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class FaceDataRepository(ApplicationDbContext context) : GenericRepository<FaceData>(context), IFaceDataRepository
{
    public Task<FaceData?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(f => f.UserId == userId, cancellationToken);
    }

    public Task<FaceData?> GetByExternalFaceIdAsync(string externalFaceId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(f => f.ExternalFaceId == externalFaceId, cancellationToken);
    }
}
