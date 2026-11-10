using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Expenses.Commands.CreateExpense;

public sealed record CreateExpenseCommand(
    string Category,
    decimal Amount,
    DateOnly ExpenseDate,
    string Description) : IRequest<Result<ExpenseDetailDto>>;
