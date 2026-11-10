using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetPayrollPeriodDetailById;

public sealed class GetPayrollPeriodDetailByIdQueryHandler(
    IPayrollRepository payrollRepository) : IRequestHandler<GetPayrollPeriodDetailByIdQuery, Result<PayrollPeriodDetailDto>>
{
    public async Task<Result<PayrollPeriodDetailDto>> Handle(GetPayrollPeriodDetailByIdQuery request, CancellationToken cancellationToken)
    {
        var period = await payrollRepository.GetPayrollPeriodDetailByIdAsync(request.Id, cancellationToken);
        if (period == null)
        {
            return Result.Failure<PayrollPeriodDetailDto>(PayrollErrors.PeriodNotFound(request.Id));
        }

        return Result.Success(period);
    }
}
