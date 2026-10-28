using GymX.Application.Modules.Users.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Users.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<Result<UserProfileDto>>;
