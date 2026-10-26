using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Entities.Employees;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class EmployeeRepository(ApplicationDbContext context) : GenericRepository<Employee>(context), IEmployeeRepository
{
    public Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);
}
