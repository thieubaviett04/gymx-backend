using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Expenses.Queries.GetExpensesPaged;

public sealed class GetExpensesPagedQueryHandler(
    IExpenseRepository expenseRepository) : IRequestHandler<GetExpensesPagedQuery, Result<PagedResult<ExpenseSummaryDto>>>
{
    public async Task<Result<PagedResult<ExpenseSummaryDto>>> Handle(GetExpensesPagedQuery request, CancellationToken cancellationToken)
    {
        var result = await expenseRepository.GetExpensesPagedAsync(
            request.Search,
            request.Category,
            request.Status,
            request.FromDate,
            request.ToDate,
            request.SortBy,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        return Result.Success(result);
    }
}
