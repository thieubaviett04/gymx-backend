using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Expenses.Queries.GetExpensesPaged;

public sealed record GetExpensesPagedQuery(
    string? Search,
    string? Category,
    string? Status,
    DateOnly? FromDate,
    DateOnly? ToDate,
    string? SortBy,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<ExpenseSummaryDto>>>;
