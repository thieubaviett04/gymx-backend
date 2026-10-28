using GymX.Application.Modules.Trainers.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Trainers.Commands.UpdateMyTrainerProfile;

public record UpdateMyTrainerProfileCommand(
    string? NationalId = null,
    string? Description = null
) : IRequest<Result<TrainerProfileDto>>;
