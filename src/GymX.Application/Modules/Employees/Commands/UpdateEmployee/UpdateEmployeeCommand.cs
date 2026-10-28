using GymX.Application.Modules.Employees.DTOs.Request;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(
    Guid EmployeeId,
    string FullName,
    string Email,
    string Position,
    decimal BaseSalaryRate,
    string EmploymentStatus,
    string? PhoneNumber = null,
    string? Gender = null,
    DateOnly? DateOfBirth = null,
    string? Address = null,
    Guid? SpecializationId = null,
    short? ExperienceYears = null,
    decimal? SalaryCoefficient = null,
    string? Description = null) : IRequest<Result<EmployeeDetailDto>>
{
    public static UpdateEmployeeCommand FromRequest(Guid id, UpdateEmployeeRequest req) => new(
        id,
        req.FullName.Trim(),
        req.Email.Trim().ToLowerInvariant(),
        req.Position.Trim().ToUpperInvariant(),
        req.BaseSalaryRate,
        string.IsNullOrWhiteSpace(req.EmploymentStatus) ? EmployeeStatus.Active : req.EmploymentStatus.Trim().ToUpperInvariant(),
        string.IsNullOrWhiteSpace(req.PhoneNumber) ? null : req.PhoneNumber.Trim(),
        string.IsNullOrWhiteSpace(req.Gender) ? null : req.Gender.Trim().ToUpperInvariant(),
        req.DateOfBirth,
        string.IsNullOrWhiteSpace(req.Address) ? null : req.Address.Trim(),
        req.SpecializationId,
        req.ExperienceYears,
        req.SalaryCoefficient,
        string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim());
}
