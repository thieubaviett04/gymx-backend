using GymX.Application.Modules.Trainers.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Trainers.Queries.GetMyTrainerProfile;

public record GetMyTrainerProfileQuery() : IRequest<Result<TrainerProfileDto>>;
