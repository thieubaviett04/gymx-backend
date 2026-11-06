using FluentValidation;

namespace GymX.Application.Modules.Packages.Commands.CreatePackage;

public class CreatePackageCommandValidator : AbstractValidator<CreatePackageCommand>
{
    public CreatePackageCommandValidator()
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
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Ngày bắt đầu áp dụng không được chọn ngày ở quá khứ.");

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
                .WithMessage("Trạng thái gói tập không hợp lệ. Chỉ chấp nhận ACTIVE hoặc INACTIVE.");
        });
    }
}
