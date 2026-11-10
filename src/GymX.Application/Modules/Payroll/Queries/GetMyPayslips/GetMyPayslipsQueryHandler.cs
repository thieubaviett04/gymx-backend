using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Queries.GetMyPayslips;

public sealed class GetMyPayslipsQueryHandler(
    IPayrollRepository payrollRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetMyPayslipsQuery, Result<IReadOnlyCollection<PayslipSummaryDto>>>
{
    public async Task<Result<IReadOnlyCollection<PayslipSummaryDto>>> Handle(GetMyPayslipsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var currentUserId))
        {
            return Result.Failure<IReadOnlyCollection<PayslipSummaryDto>>(
                Error.Unauthorized("Auth.Unauthorized", "Người dùng chưa đăng nhập hoặc thông tin xác thực không hợp lệ."));
        }

        var payslips = await payrollRepository.GetMyPayslipsAsync(currentUserId, cancellationToken);
        return Result.Success(payslips);
    }
}
