using GymX.Application.Common.Exceptions;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Expenses.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Finance;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Expenses.Commands.CreateExpense;

public sealed class CreateExpenseCommandHandler(
    IExpenseRepository expenseRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateExpenseCommand, Result<ExpenseDetailDto>>
{
    private const int MaxRetryAttempts = 3;

    public async Task<Result<ExpenseDetailDto>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var currentUserId))
        {
            return Result.Failure<ExpenseDetailDto>(Error.Unauthorized("Auth.Unauthorized", "Người dùng chưa đăng nhập hoặc thông tin xác thực không hợp lệ."));
        }

        var retryCount = 0;
        Expense? createdExpense = null;

        while (retryCount < MaxRetryAttempts)
        {
            try
            {
                var expenseCode = await expenseRepository.GenerateNextExpenseCodeAsync(request.ExpenseDate, cancellationToken);

                createdExpense = Expense.Create(
                    expenseCode,
                    request.Category,
                    request.Amount,
                    request.ExpenseDate,
                    request.Description,
                    currentUserId);

                await expenseRepository.AddAsync(createdExpense, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                break;
            }
            catch (UniqueConstraintException)
            {
                retryCount++;
                if (retryCount >= MaxRetryAttempts)
                {
                    return Result.Failure<ExpenseDetailDto>(ExpenseErrors.CodeAlreadyExists);
                }
            }
        }

        if (createdExpense == null)
        {
            return Result.Failure<ExpenseDetailDto>(ExpenseErrors.CodeAlreadyExists);
        }

        var expenseDetail = await expenseRepository.GetExpenseDetailByIdAsync(createdExpense.Id, cancellationToken);
        if (expenseDetail == null)
        {
            return Result.Failure<ExpenseDetailDto>(ExpenseErrors.NotFound(createdExpense.Id));
        }

        return Result.Success(expenseDetail);
    }
}
