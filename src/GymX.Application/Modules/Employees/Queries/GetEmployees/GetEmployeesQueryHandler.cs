using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeesQuery, Result<PagedResult<EmployeeSummaryDto>>>
{
    public async Task<Result<PagedResult<EmployeeSummaryDto>>> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
        var pageSize = request.PageSize <= 0 ? 20 : (request.PageSize > 100 ? 100 : request.PageSize);

        var result = await employeeRepository.GetEmployeesPagedAsync(
            request.Search,
            request.Position,
            request.Status,
            pageIndex,
            pageSize,
            cancellationToken);

        return Result.Success(result);
    }
}
