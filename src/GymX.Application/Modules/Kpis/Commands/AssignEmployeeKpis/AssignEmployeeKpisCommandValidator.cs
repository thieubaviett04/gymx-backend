using FluentValidation;

namespace GymX.Application.Modules.Kpis.Commands.AssignEmployeeKpis;

public sealed class AssignEmployeeKpisCommandValidator : AbstractValidator<AssignEmployeeKpisCommand>
{
    public AssignEmployeeKpisCommandValidator()
    {
        RuleFor(x => x.KpiTemplateId)
            .NotEmpty().WithMessage("Vui lòng chọn mẫu KPI.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("Tháng áp dụng không hợp lệ (1 - 12).");

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(2020).WithMessage("Năm áp dụng không hợp lệ.");

        RuleFor(x => x.EmployeeIds)
            .NotEmpty().WithMessage("Vui lòng chọn ít nhất một nhân viên.");
    }
}

