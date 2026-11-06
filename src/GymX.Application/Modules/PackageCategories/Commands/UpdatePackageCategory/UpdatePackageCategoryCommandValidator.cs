using FluentValidation;
using GymX.Domain.Constants.Packages;

namespace GymX.Application.Modules.PackageCategories.Commands.UpdatePackageCategory;

public class UpdatePackageCategoryCommandValidator : AbstractValidator<UpdatePackageCategoryCommand>
{
    public UpdatePackageCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên loại hình gói tập không được để trống.")
            .Length(2, 100).WithMessage("Tên loại hình gói tập phải từ 2 đến 100 ký tự.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả loại hình gói tập tối đa 500 ký tự.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Trạng thái không được để trống.")
            .Must(PackageCategoryStatus.IsValid).WithMessage("Trạng thái chỉ được là ACTIVE hoặc INACTIVE.");
    }
}
