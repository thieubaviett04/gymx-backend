using GymX.Domain.Entities.Employees;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Employee?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
}
