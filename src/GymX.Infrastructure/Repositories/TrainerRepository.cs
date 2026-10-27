using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Entities.Employees;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class TrainerRepository(ApplicationDbContext context) : GenericRepository<Trainer>(context), ITrainerRepository
{
    public Task<Trainer?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
        => DbSet.FirstOrDefaultAsync(t => t.EmployeeId == employeeId, cancellationToken);
}
