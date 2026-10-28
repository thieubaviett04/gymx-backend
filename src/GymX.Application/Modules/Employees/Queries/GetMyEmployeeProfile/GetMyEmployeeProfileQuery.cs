using GymX.Application.Modules.Employees.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Queries.GetMyEmployeeProfile;

public record GetMyEmployeeProfileQuery() : IRequest<Result<EmployeeProfileDto>>;
