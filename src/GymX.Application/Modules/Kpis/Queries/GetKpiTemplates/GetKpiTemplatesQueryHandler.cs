using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetKpiTemplates;

public sealed class GetKpiTemplatesQueryHandler(IKpiTemplateRepository repository)
    : IRequestHandler<GetKpiTemplatesQuery, Result<PagedResult<KpiTemplateSummaryDto>>>
{
    public async Task<Result<PagedResult<KpiTemplateSummaryDto>>> Handle(GetKpiTemplatesQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetTemplatesPagedAsync(
            request.Search, request.TargetRole, request.Status, request.PageIndex, request.PageSize, cancellationToken);
        return Result.Success(result);
    }
}

