using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDetailDto>>
{
    public async Task<Result<EmployeeDetailDto>> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var detail = await employeeRepository.GetEmployeeDetailByIdAsync(request.EmployeeId, cancellationToken);
        if (detail is null)
        {
            return Result.Failure<EmployeeDetailDto>(
                Error.NotFound("Employee.NotFound", "Không tìm thấy thông tin nhân viên."));
        }

        return Result.Success(detail);
    }
}
