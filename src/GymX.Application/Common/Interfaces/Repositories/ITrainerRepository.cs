using GymX.Domain.Entities.Employees;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface ITrainerRepository : IGenericRepository<Trainer>
{
    Task<Trainer?> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
