using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Queries.GetSalaryFormulaById;

public sealed record GetSalaryFormulaByIdQuery(Guid Id) : IRequest<Result<SalaryFormulaDto>>;
