using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Expenses.Commands.CancelExpense;

public sealed record CancelExpenseCommand(
    Guid Id,
    string CancellationReason) : IRequest<Result<ExpenseDetailDto>>;
