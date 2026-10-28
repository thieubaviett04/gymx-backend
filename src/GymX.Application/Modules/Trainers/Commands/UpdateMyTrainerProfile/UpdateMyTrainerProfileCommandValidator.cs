using FluentValidation;

namespace GymX.Application.Modules.Trainers.Commands.UpdateMyTrainerProfile;

public class UpdateMyTrainerProfileCommandValidator : AbstractValidator<UpdateMyTrainerProfileCommand>
{
    public UpdateMyTrainerProfileCommandValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.NationalId), () =>
        {
            RuleFor(x => x.NationalId!)
                .Matches(@"^(\d{9}|\d{12})$")
                .WithMessage("Số CCCD/CMND phải là 9 số (CMND cũ) hoặc đúng 12 số (CCCD).");
        });
    }
}
