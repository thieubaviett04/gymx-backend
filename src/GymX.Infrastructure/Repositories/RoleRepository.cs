using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Entities.Identity;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class RoleRepository(ApplicationDbContext context) : GenericRepository<Role>(context), IRoleRepository
{
    public Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        => Context.Roles.FirstOrDefaultAsync(r => r.Code == code, cancellationToken);
}
