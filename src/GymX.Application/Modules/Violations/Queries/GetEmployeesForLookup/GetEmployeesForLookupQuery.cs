using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Violations.Queries.GetEmployeesForLookup;

public record GetEmployeesForLookupQuery(string? Search) : IRequest<Result<List<EmployeeLookupDto>>>;

