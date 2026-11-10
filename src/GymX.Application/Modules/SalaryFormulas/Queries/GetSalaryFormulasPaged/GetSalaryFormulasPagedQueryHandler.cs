using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Queries.GetSalaryFormulasPaged;

public sealed class GetSalaryFormulasPagedQueryHandler(
    ISalaryFormulaRepository formulaRepository) : IRequestHandler<GetSalaryFormulasPagedQuery, Result<PagedResult<SalaryFormulaDto>>>
{
    public async Task<Result<PagedResult<SalaryFormulaDto>>> Handle(GetSalaryFormulasPagedQuery request, CancellationToken cancellationToken)
    {
        var result = await formulaRepository.GetFormulasPagedAsync(
            request.Search,
            request.TargetRole,
            request.Status,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        return Result.Success(result);
    }
}
