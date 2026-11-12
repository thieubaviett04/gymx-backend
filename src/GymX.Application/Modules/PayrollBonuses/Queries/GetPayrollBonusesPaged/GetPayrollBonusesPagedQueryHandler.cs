using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Queries.GetPayrollBonusesPaged;

public sealed class GetPayrollBonusesPagedQueryHandler(
    IPayrollBonusRepository bonusRepository) : IRequestHandler<GetPayrollBonusesPagedQuery, Result<PayrollBonusPagedResultDto>>
{
    public async Task<Result<PayrollBonusPagedResultDto>> Handle(
        GetPayrollBonusesPagedQuery request,
        CancellationToken cancellationToken)
    {
        var result = await bonusRepository.GetBonusesPagedAsync(
            request.Month,
            request.Year,
            request.Search,
            request.Position,
            request.BonusType,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        return Result.Success(result);
    }
}

