using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Common.Utilities;
using GymX.Application.Modules.Employees.DTOs.Response;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Employees;
using GymX.Domain.Entities.Identity;
using MediatR;

namespace GymX.Application.Modules.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ITrainerRepository trainerRepository,
    IPasswordHasher passwordHasher,
    IEmailService emailService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDetailDto>>
{
    public async Task<Result<EmployeeDetailDto>> Handle(
        CreateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        var existingEmp = await employeeRepository.GetByNationalIdAsync(command.NationalId, cancellationToken);
        var existingEmail = await userRepository.GetByEmailAsync(command.Email, cancellationToken);
        var existingPhone = command.PhoneNumber is not null
            ? await userRepository.GetByPhoneNumberAsync(command.PhoneNumber, cancellationToken)
            : null;

        if (existingEmp is not null || existingEmail is not null || existingPhone is not null)
        {
            var conflictError = existingEmp is not null
                ? Error.Conflict("Employee.NationalIdExists", "Số CCCD này đã tồn tại trong hệ thống.")
                : existingEmail is not null
                    ? Error.Conflict("User.EmailExists", "Email này đã được sử dụng trong hệ thống.")
                    : Error.Conflict("User.PhoneExists", "Số điện thoại này đã được sử dụng trong hệ thống.");

            return Result.Failure<EmployeeDetailDto>(conflictError);
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var rawPassword = PasswordGenerator.Generate(8);
            var passwordHash = passwordHasher.Hash(rawPassword);
            var employeeCode = await employeeRepository.GenerateNextEmployeeCodeAsync(cancellationToken);

            var user = User.CreateWithPassword(command.Email, command.FullName, passwordHash);
            user.VerifyEmail();
            user.UpdateProfile(command.PhoneNumber, command.Gender, command.DateOfBirth, command.Address);

            var role = await roleRepository.GetByCodeAsync(command.Position, cancellationToken);
            if (role is not null)
            {
                user.AssignRole(role.Id);
            }
            await userRepository.AddAsync(user, cancellationToken);

            var hiredAt = command.HiredAt ?? DateOnly.FromDateTime(DateTime.UtcNow);
            var employee = Employee.Create(
                user.Id,
                employeeCode,
                command.Position,
                command.BaseSalaryRate,
                hiredAt,
                command.NationalId);
            await employeeRepository.AddAsync(employee, cancellationToken);

            if (command.Position == RoleCode.Trainer)
            {
                var trainer = Trainer.Create(
                    employee.Id,
                    command.SpecializationId ?? Guid.Empty,
                    command.ExperienceYears ?? 1,
                    command.SalaryCoefficient,
                    command.Description);
                await trainerRepository.AddAsync(trainer, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var emailSubject = "Chào mừng bạn đến với GymX - Thông tin tài khoản nhân viên";
            var emailBody = $@"
                <div style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
                    <h2 style=""color: #0284c7;"">GymX Management System</h2>
                    <p>Xin chào <b>{command.FullName}</b>,</p>
                    <p>Tài khoản nhân viên của bạn tại hệ thống phòng tập GymX đã được tạo thành công:</p>
                    <table style=""border-collapse: collapse; width: 100%; max-width: 500px;"">
                        <tr><td style=""padding: 8px; border: 1px solid #ddd;""><b>Mã nhân viên:</b></td><td style=""padding: 8px; border: 1px solid #ddd;"">{employeeCode}</td></tr>
                        <tr><td style=""padding: 8px; border: 1px solid #ddd;""><b>Chức vụ:</b></td><td style=""padding: 8px; border: 1px solid #ddd;"">{command.Position}</td></tr>
                        <tr><td style=""padding: 8px; border: 1px solid #ddd;""><b>Email đăng nhập:</b></td><td style=""padding: 8px; border: 1px solid #ddd;"">{command.Email}</td></tr>
                        <tr><td style=""padding: 8px; border: 1px solid #ddd;""><b>Mật khẩu tạm thời:</b></td><td style=""padding: 8px; border: 1px solid #ddd; color: #e11d48; font-weight: bold;"">{rawPassword}</td></tr>
                    </table>
                    <p style=""margin-top: 16px;"">Vui lòng đăng nhập và đổi lại mật khẩu cá nhân ngay trong lần đầu sử dụng hệ thống.</p>
                    <p>Trân trọng,<br/><b>Ban Quản Trị GymX</b></p>
                </div>";

            var emailSent = await emailService.SendEmailAsync(command.Email, emailSubject, emailBody, cancellationToken);
            if (!emailSent)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure<EmployeeDetailDto>(
                    Error.Failure("Email.SendFailed", "Không thể gửi email kích hoạt tài khoản cho nhân viên. Giao dịch đã được hủy."));
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            var detail = await employeeRepository.GetEmployeeDetailByIdAsync(employee.Id, cancellationToken);
            return Result.Success(detail!);
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
