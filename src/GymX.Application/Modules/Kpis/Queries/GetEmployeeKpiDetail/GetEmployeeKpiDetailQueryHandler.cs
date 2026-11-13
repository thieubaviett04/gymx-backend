using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetEmployeeKpiDetail;

public sealed class GetEmployeeKpiDetailQueryHandler(IEmployeeKpiRepository repository)
    : IRequestHandler<GetEmployeeKpiDetailQuery, Result<EmployeeKpiDetailDto>>
{
    public async Task<Result<EmployeeKpiDetailDto>> Handle(GetEmployeeKpiDetailQuery request, CancellationToken cancellationToken)
    {
        var detail = await repository.GetKpiDetailByIdAsync(request.Id, cancellationToken);
        if (detail == null)
        {
            return Result.Failure<EmployeeKpiDetailDto>(KpiErrors.EmployeeKpiNotFound);
        }

        return Result.Success(detail);
    }
}

