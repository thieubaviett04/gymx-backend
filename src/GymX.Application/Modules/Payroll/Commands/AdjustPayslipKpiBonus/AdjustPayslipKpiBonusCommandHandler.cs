using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Payroll.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Payroll.Commands.AdjustPayslipKpiBonus;

public sealed class AdjustPayslipKpiBonusCommandHandler(
    IPayrollRepository payrollRepository) : IRequestHandler<AdjustPayslipKpiBonusCommand, Result<PayslipDetailDto>>
{
    public async Task<Result<PayslipDetailDto>> Handle(
        AdjustPayslipKpiBonusCommand request,
        CancellationToken cancellationToken)
    {
        if (request.KpiBonusAmount < 0)
        {
            return Result.Failure<PayslipDetailDto>(
                Error.Validation("Payroll.InvalidKpiBonusAmount", "Số tiền thưởng KPI không được nhỏ hơn 0."));
        }

        return await payrollRepository.AdjustPayslipKpiBonusAsync(
            request.PayslipId,
            request.KpiBonusAmount,
            request.Note,
            cancellationToken);
    }
}
