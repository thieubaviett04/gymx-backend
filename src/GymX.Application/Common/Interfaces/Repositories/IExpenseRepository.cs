using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Finance;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface IExpenseRepository : IGenericRepository<Expense>
{
    Task<string> GenerateNextExpenseCodeAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<bool> ExistsCodeAsync(string expenseCode, CancellationToken cancellationToken = default);
    Task<PagedResult<ExpenseSummaryDto>> GetExpensesPagedAsync(
        string? search,
        string? category,
        string? status,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? sortBy,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<ExpenseDetailDto?> GetExpenseDetailByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalExpenseInPeriodAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);
}
