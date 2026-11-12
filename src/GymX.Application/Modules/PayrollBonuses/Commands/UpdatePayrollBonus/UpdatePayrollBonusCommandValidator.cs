using FluentValidation;

namespace GymX.Application.Modules.PayrollBonuses.Commands.UpdatePayrollBonus;

public sealed class UpdatePayrollBonusCommandValidator : AbstractValidator<UpdatePayrollBonusCommand>
{
    public UpdatePayrollBonusCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Số tiền khen thưởng không được nhỏ hơn 0.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Vui lòng nhập nội dung khen thưởng.")
            .MaximumLength(500)
            .WithMessage("Nội dung khen thưởng không vượt quá 500 ký tự.");
    }
}
