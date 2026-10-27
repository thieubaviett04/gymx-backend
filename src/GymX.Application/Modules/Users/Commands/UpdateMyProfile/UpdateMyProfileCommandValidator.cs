using FluentValidation;

namespace GymX.Application.Modules.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    private static readonly string[] ValidGenders = ["MALE", "FEMALE", "OTHER", "NAM", "NỮ", "KHÁC"];

    public UpdateMyProfileCommandValidator()
    {
        When(x => x.FullName is not null, () =>
        {
            RuleFor(x => x.FullName!)
                .NotEmpty().WithMessage("Họ và tên không được để trống.")
                .MaximumLength(100).WithMessage("Họ và tên không vượt quá 100 ký tự.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber!)
                .Matches(@"^(0|\+84)[3|5|7|8|9][0-9]{8}$")
                .WithMessage("Số điện thoại không đúng định dạng Việt Nam.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Gender), () =>
        {
            RuleFor(x => x.Gender!)
                .Must(g => ValidGenders.Contains(g.Trim().ToUpperInvariant()))
                .WithMessage("Giới tính không hợp lệ. Chỉ chấp nhận: Nam, Nữ, Khác (hoặc Male, Female, Other).");
        });

        When(x => x.DateOfBirth.HasValue, () =>
        {
            RuleFor(x => x.DateOfBirth!.Value)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Ngày sinh không thể ở tương lai.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Address), () =>
        {
            RuleFor(x => x.Address!)
                .MaximumLength(255).WithMessage("Địa chỉ không vượt quá 255 ký tự.");
        });
    }
}
