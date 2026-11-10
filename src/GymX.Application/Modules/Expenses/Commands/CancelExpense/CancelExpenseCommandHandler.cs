using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Finance;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Expenses.Commands.CancelExpense;

public sealed class CancelExpenseCommandHandler(
    IExpenseRepository expenseRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelExpenseCommand, Result<ExpenseDetailDto>>
{
    public async Task<Result<ExpenseDetailDto>> Handle(CancelExpenseCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var currentUserId))
        {
            return Result.Failure<ExpenseDetailDto>(Error.Unauthorized("Auth.Unauthorized", "Người dùng chưa đăng nhập hoặc thông tin xác thực không hợp lệ."));
        }

        var expense = await expenseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (expense == null)
        {
            return Result.Failure<ExpenseDetailDto>(ExpenseErrors.NotFound(request.Id));
        }

        if (expense.Status == ExpenseStatus.Cancelled)
        {
            return Result.Failure<ExpenseDetailDto>(ExpenseErrors.AlreadyCancelled);
        }

        expense.Cancel(currentUserId, request.CancellationReason);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var expenseDetail = await expenseRepository.GetExpenseDetailByIdAsync(expense.Id, cancellationToken);
        if (expenseDetail == null)
        {
            return Result.Failure<ExpenseDetailDto>(ExpenseErrors.NotFound(expense.Id));
        }

        return Result.Success(expenseDetail);
    }
}
