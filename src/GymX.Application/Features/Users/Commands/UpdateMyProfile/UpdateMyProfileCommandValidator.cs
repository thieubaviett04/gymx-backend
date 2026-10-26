using FluentValidation;

namespace GymX.Application.Features.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    private static readonly string[] AllowedGenders = ["MALE", "FEMALE", "OTHER"];

    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên không được để trống.")
            .MaximumLength(100).WithMessage("Họ và tên không được vượt quá 100 ký tự.");

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber!)
                .Matches(@"^(0|\+84)[3|5|7|8|9][0-9]{8}$")
                .WithMessage("Số điện thoại không hợp lệ.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Gender), () =>
        {
            RuleFor(x => x.Gender!)
                .Must(g => AllowedGenders.Contains(g.ToUpperInvariant()))
                .WithMessage("Giới tính phải là MALE, FEMALE hoặc OTHER.");
        });

        When(x => x.DateOfBirth.HasValue, () =>
        {
            RuleFor(x => x.DateOfBirth!.Value)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-10)))
                .WithMessage("Ngày sinh không hợp lệ (người dùng phải từ 10 tuổi trở lên).")
                .GreaterThan(new DateOnly(1920, 1, 1))
                .WithMessage("Năm sinh không hợp lệ.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactPhone), () =>
        {
            RuleFor(x => x.EmergencyContactPhone!)
                .Matches(@"^(0|\+84)[3|5|7|8|9][0-9]{8}$")
                .WithMessage("Số điện thoại người liên hệ khẩn cấp không hợp lệ.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.NationalId), () =>
        {
            RuleFor(x => x.NationalId!)
                .Matches(@"^(\d{9}|\d{12})$")
                .WithMessage("Số CCCD/CMND phải là 9 số (CMND cũ) hoặc đúng 12 số (CCCD).");
        });
    }
}
