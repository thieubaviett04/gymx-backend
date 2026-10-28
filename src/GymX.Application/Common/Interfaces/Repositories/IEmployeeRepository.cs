using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Employees;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Employee?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
    Task<Employee?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default);
    Task<string> GenerateNextEmployeeCodeAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeSummaryDto>> GetEmployeesPagedAsync(
        string? search,
        string? position,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<EmployeeDetailDto?> GetEmployeeDetailByIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
