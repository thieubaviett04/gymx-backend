using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Finance;
using GymX.Domain.Entities.Finance;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class ExpenseRepository(ApplicationDbContext context)
    : GenericRepository<Expense>(context), IExpenseRepository
{
    public async Task<string> GenerateNextExpenseCodeAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var codePrefix = $"EXP-{date:yyyyMMdd}-";

        var latestExpenseCode = await Context.Expenses
            .AsNoTracking()
            .Where(expense => expense.ExpenseCode.StartsWith(codePrefix))
            .OrderByDescending(expense => expense.ExpenseCode)
            .Select(expense => expense.ExpenseCode)
            .FirstOrDefaultAsync(cancellationToken);

        var maxSequenceNumber = 0;
        if (latestExpenseCode != null)
        {
            var codeParts = latestExpenseCode.Split('-');
            if (codeParts.Length == 3 && int.TryParse(codeParts[2], out var sequence))
            {
                maxSequenceNumber = sequence;
            }
        }

        return $"{codePrefix}{(maxSequenceNumber + 1):D4}";
    }

    public async Task<bool> ExistsCodeAsync(string expenseCode, CancellationToken cancellationToken = default)
    {
        var normalizedCode = expenseCode.Trim().ToUpperInvariant();
        return await Context.Expenses.AnyAsync(
            expense => expense.ExpenseCode == normalizedCode,
            cancellationToken);
    }

    public async Task<PagedResult<ExpenseSummaryDto>> GetExpensesPagedAsync(
        string? search,
        string? category,
        string? status,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? sortBy,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var expenseQuery = from expense in Context.Expenses.AsNoTracking()
                            join user in Context.Users.AsNoTracking() on expense.RecordedBy equals user.Id
                            select new
                            {
                                Expense = expense,
                                RecordedUser = user
                            };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchKeyword = search.Trim();
            expenseQuery = expenseQuery.Where(item =>
                EF.Functions.ILike(item.Expense.ExpenseCode, $"%{searchKeyword}%") ||
                EF.Functions.ILike(item.Expense.Description, $"%{searchKeyword}%") ||
                EF.Functions.ILike(item.RecordedUser.FullName, $"%{searchKeyword}%"));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim().ToUpperInvariant();
            expenseQuery = expenseQuery.Where(item => item.Expense.Category == normalizedCategory);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToUpperInvariant();
            expenseQuery = expenseQuery.Where(item => item.Expense.Status == normalizedStatus);
        }

        if (fromDate.HasValue)
        {
            expenseQuery = expenseQuery.Where(item => item.Expense.ExpenseDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            expenseQuery = expenseQuery.Where(item => item.Expense.ExpenseDate <= toDate.Value);
        }

        var orderedQuery = sortBy?.ToLowerInvariant() switch
        {
            "date_asc" => expenseQuery.OrderBy(item => item.Expense.ExpenseDate).ThenBy(item => item.Expense.CreatedDate),
            "date_desc" => expenseQuery.OrderByDescending(item => item.Expense.ExpenseDate).ThenByDescending(item => item.Expense.CreatedDate),
            "amount_asc" => expenseQuery.OrderBy(item => item.Expense.Amount),
            "amount_desc" => expenseQuery.OrderByDescending(item => item.Expense.Amount),
            _ => expenseQuery.OrderByDescending(item => item.Expense.ExpenseDate).ThenByDescending(item => item.Expense.CreatedDate)
        };

        var totalCount = await orderedQuery.CountAsync(cancellationToken);

        var items = await orderedQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new ExpenseSummaryDto
            {
                Id = item.Expense.Id,
                ExpenseCode = item.Expense.ExpenseCode,
                Category = item.Expense.Category,
                Amount = item.Expense.Amount,
                ExpenseDate = item.Expense.ExpenseDate,
                Description = item.Expense.Description,
                Status = item.Expense.Status,
                RecordedByName = item.RecordedUser.FullName,
                CreatedAt = item.Expense.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ExpenseSummaryDto>(items, totalCount, pageIndex, pageSize);
    }

    public async Task<ExpenseDetailDto?> GetExpenseDetailByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expenseQuery = from expense in Context.Expenses.AsNoTracking()
                            where expense.Id == id
                            join recordedUser in Context.Users.AsNoTracking() on expense.RecordedBy equals recordedUser.Id
                            join cancelledUser in Context.Users.AsNoTracking() on expense.CancelledBy equals cancelledUser.Id into cancelledUsers
                            from cancelledUser in cancelledUsers.DefaultIfEmpty()
                            select new ExpenseDetailDto
                            {
                                Id = expense.Id,
                                ExpenseCode = expense.ExpenseCode,
                                Category = expense.Category,
                                Amount = expense.Amount,
                                ExpenseDate = expense.ExpenseDate,
                                Description = expense.Description,
                                Status = expense.Status,
                                RecordedById = expense.RecordedBy,
                                RecordedByName = recordedUser.FullName,
                                CancelledById = expense.CancelledBy,
                                CancelledByName = cancelledUser != null ? cancelledUser.FullName : null,
                                CancelledAt = expense.CancelledAt,
                                CancellationReason = expense.CancellationReason,
                                CreatedAt = expense.CreatedDate
                            };

        return await expenseQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalExpenseInPeriodAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)
    {
        return await Context.Expenses
            .AsNoTracking()
            .Where(expense => expense.Status == ExpenseStatus.Confirmed &&
                              expense.ExpenseDate >= fromDate &&
                              expense.ExpenseDate <= toDate)
            .SumAsync(expense => expense.Amount, cancellationToken);
    }
}
