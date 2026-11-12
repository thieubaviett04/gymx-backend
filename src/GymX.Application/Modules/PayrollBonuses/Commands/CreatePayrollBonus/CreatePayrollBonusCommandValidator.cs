using FluentValidation;

namespace GymX.Application.Modules.PayrollBonuses.Commands.CreatePayrollBonus;

public sealed class CreatePayrollBonusCommandValidator : AbstractValidator<CreatePayrollBonusCommand>
{
    public CreatePayrollBonusCommandValidator()
    {
        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("Tháng khen thưởng phải từ 1 đến 12.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2020, 2100)
            .WithMessage("Năm khen thưởng không hợp lệ.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Số tiền thưởng phải lớn hơn 0.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Vui lòng nhập lý do / dịp khen thưởng.")
            .MaximumLength(500)
            .WithMessage("Lý do khen thưởng không vượt quá 500 ký tự.");
    }
}
