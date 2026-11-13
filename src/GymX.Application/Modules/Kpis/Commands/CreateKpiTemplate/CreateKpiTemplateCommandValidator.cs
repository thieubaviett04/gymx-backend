using FluentValidation;
using GymX.Domain.Constants;

namespace GymX.Application.Modules.Kpis.Commands.CreateKpiTemplate;

public sealed class CreateKpiTemplateCommandValidator : AbstractValidator<CreateKpiTemplateCommand>
{
    public CreateKpiTemplateCommandValidator()
    {
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

