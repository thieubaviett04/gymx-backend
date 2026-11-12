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
            .WithMessage("Tháng khen thưởng phải từ 1 đến 12.")
            .Must((command, month) =>
            {
                if (command.Year < 2020 || command.Year > 2100 || month < 1 || month > 12)
                {
                    return true;
                }

                var now = DateTime.UtcNow.AddHours(7);
                var currentPeriod = new DateOnly(now.Year, now.Month, 1);
                var selectedPeriod = new DateOnly(command.Year, month, 1);
                return selectedPeriod >= currentPeriod;
            })
            .WithMessage("Kỳ tính lương không được là kỳ trong quá khứ. Vui lòng chọn tháng hiện tại hoặc các tháng tiếp theo.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2020, 2100)
            .WithMessage("Năm khen thưởng không hợp lệ.")
            .Must((command, year) =>
            {
                if (year < 2020 || year > 2100 || command.Month < 1 || command.Month > 12)
                {
                    return true;
                }

                var now = DateTime.UtcNow.AddHours(7);
                var currentPeriod = new DateOnly(now.Year, now.Month, 1);
                var selectedPeriod = new DateOnly(year, command.Month, 1);
                return selectedPeriod >= currentPeriod;
            })
            .WithMessage("Kỳ tính lương không được là kỳ trong quá khứ. Vui lòng chọn tháng hiện tại hoặc các tháng tiếp theo.");

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

