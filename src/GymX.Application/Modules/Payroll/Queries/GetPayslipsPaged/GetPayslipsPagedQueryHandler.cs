using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetPayslipsPaged;

public sealed class GetPayslipsPagedQueryHandler(
    IPayrollRepository payrollRepository) : IRequestHandler<GetPayslipsPagedQuery, Result<PagedResult<PayslipSummaryDto>>>
{
    public async Task<Result<PagedResult<PayslipSummaryDto>>> Handle(GetPayslipsPagedQuery request, CancellationToken cancellationToken)
    {
        var result = await payrollRepository.GetPayslipsPagedAsync(
            request.PeriodId,
            request.EmployeeId,
            request.Status,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        return Result.Success(result);
    }
}
