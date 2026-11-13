using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetKpiTemplateById;

public sealed class GetKpiTemplateByIdQueryHandler(IKpiTemplateRepository repository)
    : IRequestHandler<GetKpiTemplateByIdQuery, Result<KpiTemplateDto>>
{
    public async Task<Result<KpiTemplateDto>> Handle(GetKpiTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await repository.GetTemplateDetailByIdAsync(request.Id, cancellationToken);
        if (template == null)
        {
            return Result.Failure<KpiTemplateDto>(KpiErrors.TemplateNotFound);
        }
        return Result.Success(template);
    }
}

