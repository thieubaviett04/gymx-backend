using FluentValidation;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Identity;

namespace GymX.Application.Modules.SalaryFormulas.Commands.CreateSalaryFormula;

public sealed class CreateSalaryFormulaCommandValidator : AbstractValidator<CreateSalaryFormulaCommand>
{
    private static readonly string[] ValidRoles = [RoleCode.Trainer, RoleCode.Receptionist, "ALL"];
    private static readonly string[] ValidRateTypes = [SalaryBaseRateType.Monthly, SalaryBaseRateType.PerShift, SalaryBaseRateType.Hourly];

    public CreateSalaryFormulaCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên công thức tính lương không được để trống.")
            .MaximumLength(150).WithMessage("Tên công thức không được vượt quá 150 ký tự.");

        RuleFor(x => x.TargetRole)
            .NotEmpty().WithMessage("Vai trò áp dụng không được để trống.")
            .Must(role => ValidRoles.Contains(role.ToUpperInvariant()))
            .WithMessage("Vai trò áp dụng không hợp lệ. Chỉ chấp nhận TRAINER, RECEPTIONIST hoặc ALL.");

        RuleFor(x => x.BaseRateType)
            .NotEmpty().WithMessage("Hình thức trả lương không được để trống.")
            .Must(type => ValidRateTypes.Contains(type.ToUpperInvariant()))
            .WithMessage("Hình thức trả lương không hợp lệ. Chỉ chấp nhận MONTHLY, PER_SHIFT hoặc HOURLY.");

        RuleFor(x => x.BaseRate)
            .GreaterThanOrEqualTo(0).WithMessage("Mức lương cơ bản phải là số không âm (>= 0).");

        RuleFor(x => x.StandardWorkDays)
            .InclusiveBetween(1, 31).WithMessage("Số ngày công chuẩn trong tháng phải từ 1 đến 31 ngày.");

        RuleFor(x => x.PricePerPtSession)
            .GreaterThanOrEqualTo(0).WithMessage("Đơn giá buổi dạy PT phải là số không âm (>= 0).");

        RuleFor(x => x.SalesCommissionPercent)
            .InclusiveBetween(0, 100).WithMessage("Tỷ lệ hoa hồng bán gói phải từ 0% đến 100%.");

        RuleFor(x => x.EffectiveDate)
            .NotEmpty().WithMessage("Ngày áp dụng công thức không được để trống.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
    }
}
