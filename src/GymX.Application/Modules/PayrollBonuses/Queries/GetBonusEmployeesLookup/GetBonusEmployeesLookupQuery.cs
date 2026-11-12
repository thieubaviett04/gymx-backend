using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.PayrollBonuses.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PayrollBonuses.Queries.GetBonusEmployeesLookup;

public sealed record GetBonusEmployeesLookupQuery(
    int? Month,
    int? Year,
    string? Search) : IRequest<Result<IReadOnlyCollection<BonusEmployeeLookupDto>>>;

public sealed class GetBonusEmployeesLookupQueryHandler(
    IPayrollBonusRepository bonusRepository) : IRequestHandler<GetBonusEmployeesLookupQuery, Result<IReadOnlyCollection<BonusEmployeeLookupDto>>>
{
    public async Task<Result<IReadOnlyCollection<BonusEmployeeLookupDto>>> Handle(
        GetBonusEmployeesLookupQuery request,
        CancellationToken cancellationToken)
    {
        var list = await bonusRepository.GetEmployeesLookupAsync(
            request.Month,
            request.Year,
            request.Search,
            cancellationToken);

        return Result.Success(list);
    }
}
