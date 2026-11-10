using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.SalaryFormulas.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.SalaryFormulas.Queries.GetSalaryFormulasLookup;

public sealed class GetSalaryFormulasLookupQueryHandler(
    ISalaryFormulaRepository formulaRepository) : IRequestHandler<GetSalaryFormulasLookupQuery, Result<IReadOnlyCollection<SalaryFormulaLookupDto>>>
{
    public async Task<Result<IReadOnlyCollection<SalaryFormulaLookupDto>>> Handle(GetSalaryFormulasLookupQuery request, CancellationToken cancellationToken)
    {
        var lookup = await formulaRepository.GetLookupAsync(request.TargetRole, cancellationToken);
        return Result.Success(lookup);
    }
}
