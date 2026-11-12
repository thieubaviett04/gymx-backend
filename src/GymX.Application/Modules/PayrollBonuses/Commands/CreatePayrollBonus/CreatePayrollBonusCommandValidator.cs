using FluentValidation;

namespace GymX.Application.Modules.PayrollBonuses.Commands.CreatePayrollBonus;

public sealed class CreatePayrollBonusCommandValidator : AbstractValidator<CreatePayrollBonusCommand>
{
    public CreatePayrollBonusCommandValidator()
    {
        RuleFor(x => x.BonusType)
            .NotEmpty()
            .WithMessage("Vui lòng chọn hình thức thưởng.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage("Tháng khen thưởng phải từ 1 đến 12.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2020, 2100)
            .WithMessage("Năm khen thưởng không hợp lệ.");

        RuleFor(x => x.EmployeeIds)
            .Must(ids => ids != null && ids.Count > 0)
            .When(x => string.Equals(x.BonusType, "KPI", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(x.TargetType, "INDIVIDUAL", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Vui lòng chọn nhân viên khen thưởng.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Số tiền khen thưởng phải lớn hơn 0.");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Nội dung khen thưởng không vượt quá 500 ký tự.");
    }
}

