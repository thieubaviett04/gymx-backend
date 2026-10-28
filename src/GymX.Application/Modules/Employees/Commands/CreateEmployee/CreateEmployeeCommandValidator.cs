using FluentValidation;
using GymX.Domain.Constants;

namespace GymX.Application.Modules.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên không được để trống.")
            .MaximumLength(150).WithMessage("Họ và tên tối đa 150 ký tự.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không đúng định dạng.");

        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("Số CCCD không được để trống.")
            .Matches(@"^\d{12}$").WithMessage("Số CCCD phải bao gồm đúng 12 chữ số.");

        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Chức vụ không được để trống.")
            .Must(p => p.Equals(RoleCode.Receptionist, StringComparison.OrdinalIgnoreCase) ||
                       p.Equals(RoleCode.Trainer, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Chức vụ không hợp lệ. Vui lòng chọn Lễ tân hoặc Huấn luyện viên.");

        RuleFor(x => x.BaseSalaryRate)
            .GreaterThanOrEqualTo(0).WithMessage("Hệ lương phải là số thực không âm.");

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber!)
                .Matches(@"^0\d{9}$")
                .WithMessage("Số điện thoại phải có 10 chữ số và bắt đầu bằng số 0.");
        });

        When(x => x.Position.Equals(RoleCode.Trainer, StringComparison.OrdinalIgnoreCase), () =>
        {
            When(x => x.ExperienceYears.HasValue, () =>
            {
                RuleFor(x => x.ExperienceYears!.Value)
                    .GreaterThan((short)0)
                    .WithMessage("Năm kinh nghiệm của huấn luyện viên phải lớn hơn 0.");
            });

            When(x => x.SalaryCoefficient.HasValue, () =>
            {
                RuleFor(x => x.SalaryCoefficient!.Value)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Giá 1 buổi tập hoặc hệ số lương phải không âm.");
            });
        });
    }
}
