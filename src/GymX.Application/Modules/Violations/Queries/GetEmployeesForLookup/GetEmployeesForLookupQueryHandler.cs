using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Violations.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Violations.Queries.GetEmployeesForLookup;

public class GetEmployeesForLookupQueryHandler(IViolationRepository repository)
    : IRequestHandler<GetEmployeesForLookupQuery, Result<List<EmployeeLookupDto>>>
{
    public async Task<Result<List<EmployeeLookupDto>>> Handle(GetEmployeesForLookupQuery request, CancellationToken cancellationToken)
    {
        var list = await repository.GetEmployeesForViolationLookupAsync(request.Search, cancellationToken);
        return Result.Success(list);
    }
}

