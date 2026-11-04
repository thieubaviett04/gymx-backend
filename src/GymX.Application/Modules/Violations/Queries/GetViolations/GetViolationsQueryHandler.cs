using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Violations.Queries.GetViolations;

public class GetViolationsQueryHandler(IViolationRepository repository)
    : IRequestHandler<GetViolationsQuery, Result<PagedResult<ViolationSummaryDto>>>
{
    public async Task<Result<PagedResult<ViolationSummaryDto>>> Handle(GetViolationsQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetViolationsPagedAsync(
            request.Search,
            request.EmployeeId,
            request.PenaltyRuleId,
            request.Status,
            request.FromDate,
            request.ToDate,
            request.SortBy,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        return Result.Success(result);
    }
}

