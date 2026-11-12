using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Commands.UpdatePayrollBonus;

public sealed class UpdatePayrollBonusCommandHandler(
    IPayrollBonusRepository bonusRepository) : IRequestHandler<UpdatePayrollBonusCommand, Result<PayrollBonusDto>>
{
    public async Task<Result<PayrollBonusDto>> Handle(
        UpdatePayrollBonusCommand request,
        CancellationToken cancellationToken)
    {
        return await bonusRepository.UpdateBonusAsync(
            request.Id,
            request.Amount,
            request.Reason,
            request.BonusType,
            cancellationToken);
    }
}

