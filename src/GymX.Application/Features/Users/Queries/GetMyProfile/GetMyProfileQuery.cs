using GymX.Application.Features.Users.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Features.Users.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<Result<UserProfileDto>>;
