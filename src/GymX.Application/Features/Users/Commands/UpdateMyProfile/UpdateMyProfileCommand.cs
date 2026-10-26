using GymX.Application.Features.Users.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Features.Users.Commands.UpdateMyProfile;

public record UpdateMyProfileCommand(
    string FullName,
    string? PhoneNumber,
    string? Gender,
    DateOnly? DateOfBirth,
    string? Address,
    string? AvatarUrl,
    string? EmergencyContactName = null,
    string? EmergencyContactPhone = null,
    string? HealthNote = null,
    string? NationalId = null
) : IRequest<Result<UserProfileDto>>;
