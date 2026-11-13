using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IEmployeeKpiRepository : IGenericRepository<EmployeeKpi>
{
    Task<bool> ExistsAssignmentAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetAssignedEmployeeIdsAsync(IEnumerable<Guid> employeeIds, int month, int year, CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeKpiProgressSummaryDto>> GetKpiProgressPagedAsync(int month, int year, string? search, string? position, string? status, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    Task<EmployeeKpiDetailDto?> GetKpiDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MyKpiDto?> GetEmployeeKpiByPeriodAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);
    Task<List<EmployeeKpi>> GetActiveByTemplateIdAsync(Guid templateId, CancellationToken cancellationToken = default);
}

