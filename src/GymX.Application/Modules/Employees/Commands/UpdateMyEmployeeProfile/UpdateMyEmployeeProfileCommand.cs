using GymX.Application.Modules.Employees.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.UpdateMyEmployeeProfile;

public record UpdateMyEmployeeProfileCommand(
    string? NationalId = null
) : IRequest<Result<EmployeeProfileDto>>;
