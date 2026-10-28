using GymX.Application.Modules.Members.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Members.Commands.UpdateMyMemberProfile;

public record UpdateMyMemberProfileCommand(
    string? EmergencyContactName = null,
    string? EmergencyContactPhone = null,
    string? HealthNote = null
) : IRequest<Result<MemberProfileDto>>;
