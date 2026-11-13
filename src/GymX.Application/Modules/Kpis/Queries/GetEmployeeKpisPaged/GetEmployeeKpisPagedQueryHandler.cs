using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetEmployeeKpisPaged;

public sealed class GetEmployeeKpisPagedQueryHandler(IEmployeeKpiRepository repository)
    : IRequestHandler<GetEmployeeKpisPagedQuery, Result<PagedResult<EmployeeKpiProgressSummaryDto>>>
{
    public async Task<Result<PagedResult<EmployeeKpiProgressSummaryDto>>> Handle(GetEmployeeKpisPagedQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetKpiProgressPagedAsync(
            request.Month,
            request.Year,
            request.Search,
            request.Position,
            request.Status,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        return Result.Success(result);
    }
}

