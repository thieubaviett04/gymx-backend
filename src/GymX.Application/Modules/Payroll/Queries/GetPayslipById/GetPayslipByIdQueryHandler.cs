using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetPayslipById;

public sealed class GetPayslipByIdQueryHandler(
    IPayrollRepository payrollRepository) : IRequestHandler<GetPayslipByIdQuery, Result<PayslipDetailDto>>
{
    public async Task<Result<PayslipDetailDto>> Handle(GetPayslipByIdQuery request, CancellationToken cancellationToken)
    {
        var payslip = await payrollRepository.GetPayslipDetailByIdAsync(request.Id, cancellationToken);
        if (payslip == null)
        {
            return Result.Failure<PayslipDetailDto>(PayrollErrors.PayslipNotFound(request.Id));
        }

        return Result.Success(payslip);
    }
}
