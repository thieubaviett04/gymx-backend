using FluentValidation;

namespace GymX.Application.Modules.Packages.Commands.UpdatePackage;

public class UpdatePackageCommandValidator : AbstractValidator<UpdatePackageCommand>
{
    public UpdatePackageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên gói tập không được để trống.")
            .MaximumLength(100).WithMessage("Tên gói tập tối đa 100 ký tự.");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Vui lòng nhập giá của gói tập.")
            .GreaterThanOrEqualTo(0).WithMessage("Giá gói tập phải là số thực không âm.");

        RuleFor(x => x.DurationMonths)
            .GreaterThan(0).WithMessage("Thời hạn gói tập phải là số nguyên dương.");

        When(x => x.EffectiveEndDate.HasValue, () =>
        {
            RuleFor(x => x.EffectiveEndDate!.Value)
                .GreaterThanOrEqualTo(x => x.EffectiveStartDate)
                .WithMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu áp dụng.");
        });
    }
}
