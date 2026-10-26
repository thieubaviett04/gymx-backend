using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Features.Users.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Features.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IMemberRepository memberRepository,
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMyProfileCommand, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<UserProfileDto>(Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserProfileDto>(Error.NotFound("User.NotFound", "Không tìm thấy thông tin người dùng."));
        }

        // Kiểm tra số điện thoại trùng lặp nếu người dùng cập nhật SĐT mới
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
        {
            var existingPhone = await userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
            if (existingPhone is not null && existingPhone.Id != user.Id)
            {
                return Result.Failure<UserProfileDto>(Error.Conflict("User.PhoneExists", "Số điện thoại này đã được sử dụng bởi người khác."));
            }
        }

        // Cập nhật thông tin User
        user.UpdateProfileInfo(
            request.FullName.Trim(),
            request.PhoneNumber?.Trim(),
            request.Gender?.ToUpperInvariant(),
            request.DateOfBirth,
            request.Address?.Trim(),
            request.AvatarUrl?.Trim()
        );
        userRepository.Update(user);

        // Cập nhật thông tin Member nếu có
        var member = await memberRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (member is not null)
        {
            member.UpdateProfile(
                request.EmergencyContactName?.Trim(),
                request.EmergencyContactPhone?.Trim(),
                request.HealthNote?.Trim()
            );
            memberRepository.Update(member);
        }

        // Cập nhật thông tin Employee nếu có
        var employee = await employeeRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (employee is not null && !string.IsNullOrWhiteSpace(request.NationalId))
        {
            employee.UpdateNationalId(request.NationalId.Trim());
            employeeRepository.Update(employee);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await userRepository.GetUserRolesAsync(user.Id, cancellationToken);

        var dto = new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            AvatarUrl = user.AvatarUrl,
            Status = user.Status,
            EmailVerifiedAt = user.EmailVerifiedAt,
            Roles = roles,
            MemberInfo = member is null ? null : new MemberProfileDto
            {
                MemberCode = member.MemberCode,
                JoinDate = member.JoinDate,
                EmergencyContactName = member.EmergencyContactName,
                EmergencyContactPhone = member.EmergencyContactPhone,
                HealthNote = member.HealthNote,
                MemberStatus = member.MemberStatus
            },
            EmployeeInfo = employee is null ? null : new EmployeeProfileDto
            {
                EmployeeCode = employee.EmployeeCode,
                Position = employee.Position,
                HiredAt = employee.HiredAt,
                NationalId = employee.NationalId,
                EmploymentStatus = employee.EmploymentStatus
            }
        };

        return Result.Success(dto);
    }
}
