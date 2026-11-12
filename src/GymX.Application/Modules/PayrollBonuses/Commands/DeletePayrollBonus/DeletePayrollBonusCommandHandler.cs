using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Commands.DeletePayrollBonus;

public sealed class DeletePayrollBonusCommandHandler(
    IPayrollBonusRepository bonusRepository) : IRequestHandler<DeletePayrollBonusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeletePayrollBonusCommand request,
        CancellationToken cancellationToken)
    {
        return await bonusRepository.DeleteBonusAsync(request.Id, cancellationToken);
    }
}

