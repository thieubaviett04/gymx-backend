using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Queries.GetSalaryFormulasLookup;

public sealed record GetSalaryFormulasLookupQuery(
    string? TargetRole = null) : IRequest<Result<IReadOnlyCollection<SalaryFormulaLookupDto>>>;
