using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Employees;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ITrainerRepository trainerRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeCommand, Result<EmployeeDetailDto>>
{
    public async Task<Result<EmployeeDetailDto>> Handle(
        UpdateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(command.EmployeeId, cancellationToken);
        if (employee is null || employee.IsDeleted || employee.Position == "ADMIN")
        {
            return Result.Failure<EmployeeDetailDto>(
                Error.NotFound("Employee.NotFound", "Không tìm thấy thông tin nhân viên."));
        }

        var user = await userRepository.GetUserWithRolesByIdAsync(employee.UserId, cancellationToken);
        if (user is null || user.IsDeleted)
        {
            return Result.Failure<EmployeeDetailDto>(
                Error.NotFound("User.NotFound", "Không tìm thấy tài khoản người dùng của nhân viên."));
        }

        var emailChanged = !string.Equals(user.Email, command.Email, StringComparison.OrdinalIgnoreCase);
        var existingEmailUser = emailChanged ? await userRepository.GetByEmailAsync(command.Email, cancellationToken) : null;

        var phoneChanged = command.PhoneNumber is not null && !string.Equals(user.PhoneNumber, command.PhoneNumber, StringComparison.OrdinalIgnoreCase);
        var existingPhoneUser = phoneChanged ? await userRepository.GetByPhoneNumberAsync(command.PhoneNumber!, cancellationToken) : null;

        var emailConflict = existingEmailUser is not null && existingEmailUser.Id != user.Id && !existingEmailUser.IsDeleted;
        var phoneConflict = existingPhoneUser is not null && existingPhoneUser.Id != user.Id && !existingPhoneUser.IsDeleted;

        if (emailConflict || phoneConflict)
        {
            var conflictError = emailConflict
                ? Error.Conflict("User.EmailExists", "Email này đã được sử dụng trong hệ thống.")
                : Error.Conflict("User.PhoneExists", "Số điện thoại này đã được sử dụng trong hệ thống.");

            return Result.Failure<EmployeeDetailDto>(conflictError);
        }

        if (emailChanged)
        {
            user.UpdateEmail(command.Email);
        }

        user.UpdateProfileInfo(command.FullName, command.PhoneNumber, command.Gender, command.DateOfBirth, command.Address);

        if (!string.Equals(employee.Position, command.Position, StringComparison.OrdinalIgnoreCase))
        {
            var oldRole = await roleRepository.GetByCodeAsync(employee.Position, cancellationToken);
            if (oldRole is not null)
            {
                user.RemoveRole(oldRole.Id);
            }

            var newRole = await roleRepository.GetByCodeAsync(command.Position, cancellationToken);
            if (newRole is not null)
            {
                user.AssignRole(newRole.Id);
            }
        }

        employee.UpdateInfo(command.Position, command.BaseSalaryRate);

        if (command.Position == RoleCode.Trainer)
        {
            var trainer = await trainerRepository.GetByEmployeeIdAsync(employee.Id, cancellationToken);
            if (trainer is not null)
            {
                trainer.UpdateTrainerInfo(
                    command.SpecializationId ?? trainer.SpecializationId,
                    command.ExperienceYears ?? trainer.ExperienceYears,
                    command.SalaryCoefficient,
                    command.Description);
                trainerRepository.Update(trainer);
            }
            else
            {
                var newTrainer = Trainer.Create(
                    employee.Id,
                    command.SpecializationId ?? Guid.Empty,
                    command.ExperienceYears ?? 1,
                    command.SalaryCoefficient,
                    command.Description);
                await trainerRepository.AddAsync(newTrainer, cancellationToken);
            }
        }

        userRepository.Update(user);
        employeeRepository.Update(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var detail = await employeeRepository.GetEmployeeDetailByIdAsync(employee.Id, cancellationToken);
        return Result.Success(detail!);
    }
}
