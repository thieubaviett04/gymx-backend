using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Queries.GetSalaryFormulasPaged;

public sealed record GetSalaryFormulasPagedQuery(
    string? Search,
    string? TargetRole,
    string? Status,
    int PageIndex = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<SalaryFormulaDto>>>;
