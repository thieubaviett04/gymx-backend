using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid EmployeeId) : IRequest<Result<EmployeeDetailDto>>;
