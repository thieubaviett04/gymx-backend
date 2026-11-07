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

        RuleFor(x => x.EffectiveStartDate)
            .Must((command, effectiveStartDate) =>
            {
                if (command.Status == GymX.Domain.Constants.PackageStatus.Pending)
                {
                    return effectiveStartDate > DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
                }
                return true;
            })
            .WithName("effectiveStartDate")
            .WithMessage("Trạng thái chưa áp dụng không thể chọn ngày hiện tại, quá khứ.");

        RuleFor(x => x.EffectiveStartDate)
            .Must((command, effectiveStartDate) =>
            {
                if (command.Status == GymX.Domain.Constants.PackageStatus.Active)
                {
                    return effectiveStartDate <= DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
                }
                return true;
            })
            .WithName("effectiveStartDate")
            .WithMessage("Trạng thái đang áp dụng không thể chọn ngày tương lai.");

        When(x => x.CategoryId.HasValue, () =>
        {
            RuleFor(x => x.CategoryId!.Value)
                .NotEmpty()
                .WithMessage("Mã loại hình gói tập không hợp lệ.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Status), () =>
        {
            RuleFor(x => x.Status!)
                .Must(GymX.Domain.Constants.PackageStatus.IsValid)
                .WithMessage("Trạng thái gói tập không hợp lệ (ACTIVE, PENDING, INACTIVE, DISCONTINUED).");
        });
    }
}
