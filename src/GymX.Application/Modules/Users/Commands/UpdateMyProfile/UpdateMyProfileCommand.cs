using GymX.Application.Modules.Users.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Users.Commands.UpdateMyProfile;

public record UpdateMyProfileCommand(
    string? FullName = null,
    string? PhoneNumber = null,
    string? Gender = null,
    DateOnly? DateOfBirth = null,
    string? Address = null
) : IRequest<Result<UserProfileDto>>;
