using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Commands.CalculatePayroll;

public sealed class CalculatePayrollCommandHandler(
    IPayrollRepository payrollRepository) : IRequestHandler<CalculatePayrollCommand, Result<PayrollPeriodDetailDto>>
{
    public async Task<Result<PayrollPeriodDetailDto>> Handle(CalculatePayrollCommand request, CancellationToken cancellationToken)
    {
        if (request.Month < 1 || request.Month > 12)
        {
            return Result.Failure<PayrollPeriodDetailDto>(
                Error.Validation("Payroll.InvalidMonth", "Tháng tính lương phải từ 1 đến 12."));
        }

        if (request.Year < 2020 || request.Year > 2100)
        {
            return Result.Failure<PayrollPeriodDetailDto>(
                Error.Validation("Payroll.InvalidYear", "Năm tính lương không hợp lệ."));
        }

        return await payrollRepository.CalculatePayrollAsync(request.Month, request.Year, cancellationToken);
    }
}
