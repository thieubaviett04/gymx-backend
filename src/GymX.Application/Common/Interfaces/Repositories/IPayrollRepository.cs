using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IPayrollRepository : IGenericRepository<PayrollPeriod>
{
    Task<Result<PayrollPeriodDetailDto>> CalculatePayrollAsync(int month, int year, CancellationToken cancellationToken = default);
    Task<PayrollPeriod?> GetPeriodByDateRangeAsync(DateOnly periodStart, DateOnly periodEnd, CancellationToken cancellationToken = default);
    Task<PagedResult<PayrollPeriodSummaryDto>> GetPayrollPeriodsPagedAsync(
        int? year,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<PayrollPeriodDetailDto?> GetPayrollPeriodDetailByIdAsync(Guid periodId, CancellationToken cancellationToken = default);
    Task<PagedResult<PayslipSummaryDto>> GetPayslipsPagedAsync(
        Guid? periodId,
        Guid? employeeId,
        string? status,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<PayslipDetailDto?> GetPayslipDetailByIdAsync(Guid payslipId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PayslipSummaryDto>> GetMyPayslipsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Payslip>> GetPayslipsByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default);
}
