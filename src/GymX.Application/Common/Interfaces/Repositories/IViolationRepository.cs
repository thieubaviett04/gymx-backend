using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IViolationRepository : IGenericRepository<Violation>
{
    Task<string> GenerateNextViolationCodeAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<bool> ExistsCodeAsync(string violationCode, CancellationToken cancellationToken = default);
    Task<bool> IsDateInClosedPayrollPeriodAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<PagedResult<ViolationSummaryDto>> GetViolationsPagedAsync(
        string? search,
        Guid? employeeId,
        Guid? penaltyRuleId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        string? sortBy,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<ViolationDetailDto?> GetViolationDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<EmployeeLookupDto>> GetEmployeesForViolationLookupAsync(string? search, CancellationToken cancellationToken = default);
}

