using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Queries.GetSalaryFormulaById;

public sealed class GetSalaryFormulaByIdQueryHandler(
    ISalaryFormulaRepository formulaRepository) : IRequestHandler<GetSalaryFormulaByIdQuery, Result<SalaryFormulaDto>>
{
    public async Task<Result<SalaryFormulaDto>> Handle(GetSalaryFormulaByIdQuery request, CancellationToken cancellationToken)
    {
        var formula = await formulaRepository.GetFormulaDetailByIdAsync(request.Id, cancellationToken);
        if (formula == null)
        {
            return Result.Failure<SalaryFormulaDto>(SalaryFormulaErrors.NotFound(request.Id));
        }

        return Result.Success(formula);
    }
}
