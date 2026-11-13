using FluentValidation;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Payroll;

namespace GymX.Application.Modules.Kpis.Commands.UpdateKpiTemplate;

public sealed class UpdateKpiTemplateCommandValidator : AbstractValidator<UpdateKpiTemplateCommand>
{
    public UpdateKpiTemplateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id mẫu KPI không được để trống.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên mẫu KPI không được để trống.")
            .MaximumLength(150).WithMessage("Tên mẫu KPI tối đa 150 ký tự.");

        RuleFor(x => x.TargetRole)
            .NotEmpty().WithMessage("Đối tượng áp dụng không được để trống.")
            .Must(role => role == RoleCode.Trainer || role == RoleCode.Receptionist)
            .WithMessage("Đối tượng áp dụng phải là TRAINER hoặc RECEPTIONIST.");

        RuleFor(x => x.TargetRevenue)
            .GreaterThanOrEqualTo(0).WithMessage("Chỉ tiêu doanh số phải là số thực không âm.");

        RuleFor(x => x.TargetBonusAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Mức thưởng định mức phải là số thực không âm.");

        RuleFor(x => x.Status)
            .Must(KpiTemplateStatus.IsValid).WithMessage("Trạng thái mẫu KPI không hợp lệ (ACTIVE hoặc INACTIVE).");

        When(x => x.TargetRole == RoleCode.Trainer, () =>
        {
            RuleFor(x => x.TargetPtSessions)
                .NotNull().WithMessage("Chỉ tiêu buổi dạy PT là bắt buộc đối với Huấn luyện viên.")
                .GreaterThanOrEqualTo(0).WithMessage("Chỉ tiêu buổi dạy PT không được âm.");
        });

        When(x => x.TargetRole == RoleCode.Receptionist, () =>
        {
            RuleFor(x => x.TargetNewMembers)
                .NotNull().WithMessage("Chỉ tiêu số hội viên mới là bắt buộc đối với Lễ tân.")
                .GreaterThanOrEqualTo(0).WithMessage("Chỉ tiêu số hội viên mới không được âm.");
        });
    }
}

