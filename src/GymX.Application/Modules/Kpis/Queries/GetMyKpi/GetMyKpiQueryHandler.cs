using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Kpis.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Errors;
using MediatR;

namespace GymX.Application.Modules.Kpis.Queries.GetMyKpi;

public sealed class GetMyKpiQueryHandler(
    IEmployeeKpiRepository employeeKpiRepository,
    IEmployeeRepository employeeRepository) : IRequestHandler<GetMyKpiQuery, Result<MyKpiDto>>
{
    public async Task<Result<MyKpiDto>> Handle(GetMyKpiQuery request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (employee == null || employee.IsDeleted)
        {
            return Result.Failure<MyKpiDto>(Error.NotFound("Employee.NotFound", "Không tìm thấy thông tin nhân viên của tài khoản này."));
        }

        var kpi = await employeeKpiRepository.GetEmployeeKpiByPeriodAsync(
            employee.Id, request.Month, request.Year, cancellationToken);

        if (kpi == null)
        {
            return Result.Failure<MyKpiDto>(KpiErrors.EmployeeKpiNotFound);
        }

        return Result.Success(kpi);
    }
}

