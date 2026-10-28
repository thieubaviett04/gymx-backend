using GymX.Application.Modules.Trainers.DTOs.Request;
using GymX.Application.Modules.Trainers.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Trainers.Commands.UpdateMyTrainerProfile;

public record UpdateMyTrainerProfileCommand(
    string? NationalId = null,
    string? Description = null
) : IRequest<Result<TrainerProfileDto>>
{
    public static UpdateMyTrainerProfileCommand FromRequest(UpdateMyTrainerProfileRequest req) => new(
        string.IsNullOrWhiteSpace(req.NationalId) ? null : req.NationalId.Trim(),
        string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim());
}
