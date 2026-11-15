using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Queries.GetEmployeeFaceByCode;

public record GetEmployeeFaceByCodeQuery(string EmployeeCode) : IRequest<Result<EmployeeFaceDetailDto>>;

