using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Expenses.Queries.GetExpenseById;

public sealed record GetExpenseByIdQuery(Guid Id) : IRequest<Result<ExpenseDetailDto>>;
