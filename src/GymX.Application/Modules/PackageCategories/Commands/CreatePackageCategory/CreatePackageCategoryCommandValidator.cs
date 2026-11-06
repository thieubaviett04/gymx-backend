using FluentValidation;

namespace GymX.Application.Modules.PackageCategories.Commands.CreatePackageCategory;

public class CreatePackageCategoryCommandValidator : AbstractValidator<CreatePackageCategoryCommand>
{
    public CreatePackageCategoryCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Mã loại hình gói tập không được để trống.")
            .MaximumLength(50).WithMessage("Mã loại hình gói tập tối đa 50 ký tự.")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Mã loại hình chỉ được chứa chữ cái, chữ số, dấu gạch dưới hoặc gạch ngang.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên loại hình gói tập không được để trống.")
            .Length(2, 100).WithMessage("Tên loại hình gói tập phải từ 2 đến 100 ký tự.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả loại hình gói tập tối đa 500 ký tự.");
    }
}
