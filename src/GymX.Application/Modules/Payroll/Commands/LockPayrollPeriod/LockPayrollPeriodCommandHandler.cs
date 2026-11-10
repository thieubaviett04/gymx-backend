using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Payroll.Commands.LockPayrollPeriod;

public sealed class LockPayrollPeriodCommandHandler(
    IPayrollRepository payrollRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : IRequestHandler<LockPayrollPeriodCommand, Result<PayrollPeriodDetailDto>>
{
    public async Task<Result<PayrollPeriodDetailDto>> Handle(LockPayrollPeriodCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var currentUserId))
        {
            return Result.Failure<PayrollPeriodDetailDto>(
                Error.Unauthorized("Auth.Unauthorized", "Người dùng chưa đăng nhập hoặc thông tin xác thực không hợp lệ."));
        }

        var period = await payrollRepository.GetByIdAsync(request.PeriodId, cancellationToken);
        if (period == null)
        {
            return Result.Failure<PayrollPeriodDetailDto>(PayrollErrors.PeriodNotFound(request.PeriodId));
        }

        if (period.Status == PayrollPeriodStatus.Confirmed)
        {
            return Result.Failure<PayrollPeriodDetailDto>(PayrollErrors.PeriodAlreadyConfirmed);
        }

        period.Confirm(currentUserId);

        var payslips = await payrollRepository.GetPayslipsByPeriodIdAsync(period.Id, cancellationToken);
        foreach (var payslip in payslips)
        {
            payslip.FinalizePayslip();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var detail = await payrollRepository.GetPayrollPeriodDetailByIdAsync(period.Id, cancellationToken);
        if (detail == null)
        {
            return Result.Failure<PayrollPeriodDetailDto>(PayrollErrors.PeriodNotFound(period.Id));
        }

        return Result.Success(detail);
    }
}
