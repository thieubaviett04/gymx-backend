using FluentValidation;

namespace GymX.Application.Modules.Members.Commands.UpdateMyMemberProfile;

public class UpdateMyMemberProfileCommandValidator : AbstractValidator<UpdateMyMemberProfileCommand>
{
    public UpdateMyMemberProfileCommandValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactPhone), () =>
        {
            RuleFor(x => x.EmergencyContactPhone!)
                .Matches(@"^(0|\+84)[3|5|7|8|9][0-9]{8}$")
                .WithMessage("Số điện thoại người liên hệ khẩn cấp không hợp lệ.");
        });
    }
}
