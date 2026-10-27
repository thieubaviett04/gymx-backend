using FluentValidation;

namespace GymX.Application.Modules.Employees.Commands.UpdateMyEmployeeProfile;

public class UpdateMyEmployeeProfileCommandValidator : AbstractValidator<UpdateMyEmployeeProfileCommand>
{
    public UpdateMyEmployeeProfileCommandValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.NationalId), () =>
        {
            RuleFor(x => x.NationalId!)
                .Matches(@"^(\d{9}|\d{12})$")
                .WithMessage("Số CCCD/CMND phải là 9 số (CMND cũ) hoặc đúng 12 số (CCCD).");
        });
    }
}
