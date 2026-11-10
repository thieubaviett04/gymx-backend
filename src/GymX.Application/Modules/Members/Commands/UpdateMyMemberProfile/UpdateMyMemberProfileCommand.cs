using GymX.Application.Modules.Members.DTOs.Request;
using GymX.Application.Modules.Members.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Members.Commands.UpdateMyMemberProfile;

public record UpdateMyMemberProfileCommand(
    string? EmergencyContactName = null,
    string? EmergencyContactPhone = null,
    string? HealthNote = null
) : IRequest<Result<MemberProfileDto>>
{
    public static UpdateMyMemberProfileCommand FromRequest(UpdateMyMemberProfileRequest req) => new(
        string.IsNullOrWhiteSpace(req.EmergencyContactName) ? null : req.EmergencyContactName.Trim(),
        string.IsNullOrWhiteSpace(req.EmergencyContactPhone) ? null : req.EmergencyContactPhone.Trim(),
        string.IsNullOrWhiteSpace(req.HealthNote) ? null : req.HealthNote.Trim());
}
