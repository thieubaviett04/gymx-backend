using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteEmployeeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(command.EmployeeId, cancellationToken);
        if (employee is null || employee.IsDeleted || employee.Position == "ADMIN")
        {
            return Result.Failure<bool>(
                Error.NotFound("Employee.NotFound", "Không tìm thấy thông tin nhân viên."));
        }

        var user = await userRepository.GetByIdAsync(employee.UserId, cancellationToken);

        employee.SoftDelete();
        employeeRepository.Update(employee);

        if (user is not null)
        {
            user.Deactivate();
            userRepository.Update(user);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
