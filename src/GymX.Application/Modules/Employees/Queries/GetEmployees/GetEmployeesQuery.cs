using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Queries.GetEmployees;

public record GetEmployeesQuery(
    string? Search = null,
    string? Position = null,
    string? Status = null,
    int PageIndex = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<EmployeeSummaryDto>>>;
