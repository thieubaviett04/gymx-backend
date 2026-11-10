using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Queries.GetMyEmployeeProfile;

public class GetMyEmployeeProfileQueryHandler(
    ICurrentUserService currentUserService,
    IEmployeeRepository employeeRepository)
    : IRequestHandler<GetMyEmployeeProfileQuery, Result<EmployeeProfileDto>>
{
    public async Task<Result<EmployeeProfileDto>> Handle(GetMyEmployeeProfileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<EmployeeProfileDto>(Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var employeeDto = await employeeRepository.GetProfileByUserIdAsync(userId, cancellationToken);
        if (employeeDto is null)
        {
            return Result.Failure<EmployeeProfileDto>(Error.NotFound("Employee.NotFound", "Không tìm thấy hồ sơ nhân viên của bạn."));
        }

        return Result.Success(employeeDto);
    }
}
