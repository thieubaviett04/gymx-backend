using GymX.Application.Modules.Employees.DTOs.Request;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    string FullName,
    string Email,
    string NationalId,
    string Position,
    decimal BaseSalaryRate,
    string? PhoneNumber = null,
    string? Gender = null,
    DateOnly? DateOfBirth = null,
    string? Address = null,
    DateOnly? HiredAt = null,
    Guid? SpecializationId = null,
    short? ExperienceYears = null,
    decimal? SalaryCoefficient = null,
    string? Description = null) : IRequest<Result<EmployeeDetailDto>>
{
    public static CreateEmployeeCommand FromRequest(CreateEmployeeRequest req) => new(
        req.FullName.Trim(),
        req.Email.Trim().ToLowerInvariant(),
        req.NationalId.Trim(),
        req.Position.Trim().ToUpperInvariant(),
        req.BaseSalaryRate,
        string.IsNullOrWhiteSpace(req.PhoneNumber) ? null : req.PhoneNumber.Trim(),
        string.IsNullOrWhiteSpace(req.Gender) ? null : req.Gender.Trim().ToUpperInvariant(),
        req.DateOfBirth,
        string.IsNullOrWhiteSpace(req.Address) ? null : req.Address.Trim(),
        req.HiredAt,
        req.SpecializationId,
        req.ExperienceYears,
        req.SalaryCoefficient,
        string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim());
}
