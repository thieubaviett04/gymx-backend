using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Payroll;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IPayrollBonusRepository : IGenericRepository<PayrollBonus>
{
    Task<PayrollBonusPagedResultDto> GetBonusesPagedAsync(
        int? month,
        int? year,
        string? search,
        string? position,
        string? bonusType,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<int>> CreateBonusesAsync(
        int month,
        int year,
        string bonusType,
        string targetType,
        string? position,
        List<Guid>? employeeIds,
        decimal amount,
        string reason,
        CancellationToken cancellationToken = default);

    Task<Result<PayrollBonusDto>> UpdateBonusAsync(
        Guid id,
        decimal amount,
        string reason,
        string bonusType,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteBonusAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BonusEmployeeLookupDto>> GetEmployeesLookupAsync(
        int? month,
        int? year,
        string? search,
        CancellationToken cancellationToken = default);
}

