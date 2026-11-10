using FluentValidation;

namespace GymX.Application.Modules.Expenses.Commands.CancelExpense;

public sealed class CancelExpenseCommandValidator : AbstractValidator<CancelExpenseCommand>
{
    public CancelExpenseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Mã định danh phiếu chi không được để trống.");

        RuleFor(x => x.CancellationReason)
            .NotEmpty().WithMessage("Bắt buộc phải nhập lý do khi hủy phiếu chi.")
            .MaximumLength(500).WithMessage("Lý do hủy không được vượt quá 500 ký tự.");
    }
}
