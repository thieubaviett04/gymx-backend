using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Features.Users.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Features.Users.Queries.GetMyProfile;

public class GetMyProfileQueryHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IMemberRepository memberRepository,
    IEmployeeRepository employeeRepository)
    : IRequestHandler<GetMyProfileQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
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

        var roles = await userRepository.GetUserRolesAsync(userId, cancellationToken);
        var member = await memberRepository.GetByUserIdAsync(userId, cancellationToken);
        var employee = await employeeRepository.GetByUserIdAsync(userId, cancellationToken);

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
