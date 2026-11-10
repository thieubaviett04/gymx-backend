using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetPayrollPeriodsPaged;

public sealed class GetPayrollPeriodsPagedQueryHandler(
    IPayrollRepository payrollRepository) : IRequestHandler<GetPayrollPeriodsPagedQuery, Result<PagedResult<PayrollPeriodSummaryDto>>>
{
    public async Task<Result<PagedResult<PayrollPeriodSummaryDto>>> Handle(GetPayrollPeriodsPagedQuery request, CancellationToken cancellationToken)
    {
        var result = await payrollRepository.GetPayrollPeriodsPagedAsync(
            request.Year,
            request.Status,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        return Result.Success(result);
    }
}
