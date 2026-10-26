using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Entities.Members;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class MemberRepository(ApplicationDbContext context) : GenericRepository<Member>(context), IMemberRepository
{
    public Task<Member?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);
}
