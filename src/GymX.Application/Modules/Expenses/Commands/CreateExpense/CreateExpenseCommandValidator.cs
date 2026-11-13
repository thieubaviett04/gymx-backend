using FluentValidation;
using GymX.Domain.Constants.Finance;

namespace GymX.Application.Modules.Expenses.Commands.CreateExpense;

public sealed class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Danh mục chi phí không được để trống.")
            .Must(ExpenseCategory.IsValid).WithMessage("Danh mục chi phí không hợp lệ.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Số tiền chi phải là số dương lớn hơn 0.");

        RuleFor(x => x.ExpenseDate)
            .NotEmpty().WithMessage("Ngày phát sinh chi phí không được để trống.")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)).AddMonths(1))
            .WithMessage("Ngày phát sinh chi phí không được vượt quá 1 tháng tới.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Nội dung chi phí không được để trống.")
            .MaximumLength(500).WithMessage("Nội dung chi phí không được vượt quá 500 ký tự.");
    }
}
