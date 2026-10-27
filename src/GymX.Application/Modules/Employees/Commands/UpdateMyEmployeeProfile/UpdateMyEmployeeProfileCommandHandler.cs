using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.UpdateMyEmployeeProfile;

public class UpdateMyEmployeeProfileCommandHandler(
    ICurrentUserService currentUserService,
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMyEmployeeProfileCommand, Result<EmployeeProfileDto>>
{
    public async Task<Result<EmployeeProfileDto>> Handle(UpdateMyEmployeeProfileCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<EmployeeProfileDto>(Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var employee = await employeeRepository.GetByUserIdAsync(userId, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<EmployeeProfileDto>(Error.NotFound("Employee.NotFound", "Không tìm thấy hồ sơ nhân viên của bạn."));
        }

        if (!string.IsNullOrWhiteSpace(request.NationalId))
        {
            var trimmedNationalId = request.NationalId.Trim();
            var existingWithNationalId = await employeeRepository.GetByNationalIdAsync(trimmedNationalId, cancellationToken);
            if (existingWithNationalId is not null && existingWithNationalId.Id != employee.Id)
            {
                return Result.Failure<EmployeeProfileDto>(Error.Conflict("Employee.NationalIdExists", "Số CCCD/CMND này đã được sử dụng bởi một nhân viên khác."));
            }

            employee.UpdateNationalId(trimmedNationalId);
            employeeRepository.Update(employee);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var employeeDto = new EmployeeProfileDto
        {
            EmployeeCode = employee.EmployeeCode,
            Position = employee.Position,
            HiredAt = employee.HiredAt,
            NationalId = employee.NationalId,
            EmploymentStatus = employee.EmploymentStatus
        };

        return Result.Success(employeeDto);
    }
}
