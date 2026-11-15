using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Queries.GetEmployeeFaceDetail;

public record GetEmployeeFaceDetailQuery(Guid EmployeeId) : IRequest<Result<EmployeeFaceDetailDto>>;
