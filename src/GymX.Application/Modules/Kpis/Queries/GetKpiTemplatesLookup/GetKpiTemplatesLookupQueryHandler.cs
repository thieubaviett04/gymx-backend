using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetKpiTemplatesLookup;

public sealed class GetKpiTemplatesLookupQueryHandler(IKpiTemplateRepository repository)
    : IRequestHandler<GetKpiTemplatesLookupQuery, Result<List<KpiTemplateLookupDto>>>
{
    public async Task<Result<List<KpiTemplateLookupDto>>> Handle(GetKpiTemplatesLookupQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetLookupAsync(request.TargetRole, cancellationToken);
        return Result.Success(result);
    }
}

