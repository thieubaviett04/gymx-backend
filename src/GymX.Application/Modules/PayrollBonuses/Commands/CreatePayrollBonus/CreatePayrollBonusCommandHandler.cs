using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Commands.CreatePayrollBonus;

public sealed class CreatePayrollBonusCommandHandler(
    IPayrollBonusRepository bonusRepository) : IRequestHandler<CreatePayrollBonusCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreatePayrollBonusCommand request,
        CancellationToken cancellationToken)
    {
        return await bonusRepository.CreateBonusesAsync(
            request.Month,
            request.Year,
            request.BonusType,
            request.TargetType,
            request.Position,
            request.EmployeeIds,
            request.Amount,
            request.Reason,
            cancellationToken);
    }
}

