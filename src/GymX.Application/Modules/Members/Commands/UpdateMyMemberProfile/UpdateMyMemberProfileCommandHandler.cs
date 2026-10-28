using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Members.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Members.Commands.UpdateMyMemberProfile;

public class UpdateMyMemberProfileCommandHandler(
    ICurrentUserService currentUserService,
    IMemberRepository memberRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMyMemberProfileCommand, Result<MemberProfileDto>>
{
    public async Task<Result<MemberProfileDto>> Handle(UpdateMyMemberProfileCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<MemberProfileDto>(Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var member = await memberRepository.GetByUserIdAsync(userId, cancellationToken);
        if (member is null)
        {
            return Result.Failure<MemberProfileDto>(Error.NotFound("Member.NotFound", "Không tìm thấy hồ sơ hội viên của bạn."));
        }

        member.UpdatePartialProfile(
            request.EmergencyContactName,
            request.EmergencyContactPhone,
            request.HealthNote
        );
        memberRepository.Update(member);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var memberDto = new MemberProfileDto
        {
            MemberCode = member.MemberCode,
            JoinDate = member.JoinDate,
            EmergencyContactName = member.EmergencyContactName,
            EmergencyContactPhone = member.EmergencyContactPhone,
            HealthNote = member.HealthNote,
            MemberStatus = member.MemberStatus
        };

        return Result.Success(memberDto);
    }
}
