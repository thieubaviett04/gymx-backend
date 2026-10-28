using GymX.Application.Modules.Users.DTOs.Request;
using GymX.Application.Modules.Users.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Users.Commands.UpdateMyProfile;

public record UpdateMyProfileCommand(
    string? FullName = null,
    string? PhoneNumber = null,
    string? Gender = null,
    DateOnly? DateOfBirth = null,
    string? Address = null
) : IRequest<Result<UserProfileDto>>
{
    public static UpdateMyProfileCommand FromRequest(UpdateMyProfileRequest req) => new(
        string.IsNullOrWhiteSpace(req.FullName) ? null : req.FullName.Trim(),
        string.IsNullOrWhiteSpace(req.PhoneNumber) ? null : req.PhoneNumber.Trim(),
        string.IsNullOrWhiteSpace(req.Gender) ? null : req.Gender.Trim().ToUpperInvariant(),
        req.DateOfBirth,
        string.IsNullOrWhiteSpace(req.Address) ? null : req.Address.Trim());
}
