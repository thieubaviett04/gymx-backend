using GymX.Application.Modules.Employees.DTOs.Request;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.UpdateMyEmployeeProfile;

public record UpdateMyEmployeeProfileCommand(
    string? NationalId = null
) : IRequest<Result<EmployeeProfileDto>>
{
    public static UpdateMyEmployeeProfileCommand FromRequest(UpdateMyEmployeeProfileRequest req) => new(
        string.IsNullOrWhiteSpace(req.NationalId) ? null : req.NationalId.Trim());
}
