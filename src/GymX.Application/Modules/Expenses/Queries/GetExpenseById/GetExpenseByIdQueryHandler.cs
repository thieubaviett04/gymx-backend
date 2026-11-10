using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Expenses.Queries.GetExpenseById;

public sealed class GetExpenseByIdQueryHandler(
    IExpenseRepository expenseRepository) : IRequestHandler<GetExpenseByIdQuery, Result<ExpenseDetailDto>>
{
    public async Task<Result<ExpenseDetailDto>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
    {
        var expense = await expenseRepository.GetExpenseDetailByIdAsync(request.Id, cancellationToken);
        if (expense == null)
        {
            return Result.Failure<ExpenseDetailDto>(ExpenseErrors.NotFound(request.Id));
        }

        return Result.Success(expense);
    }
}
