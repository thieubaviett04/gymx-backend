using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Members.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Members.Queries.GetMyMemberProfile;

public class GetMyMemberProfileQueryHandler(
    ICurrentUserService currentUserService,
    IMemberRepository memberRepository)
    : IRequestHandler<GetMyMemberProfileQuery, Result<MemberProfileDto>>
{
    public async Task<Result<MemberProfileDto>> Handle(GetMyMemberProfileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<MemberProfileDto>(Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var memberDto = await memberRepository.GetProfileByUserIdAsync(userId, cancellationToken);
        if (memberDto is null)
        {
            return Result.Failure<MemberProfileDto>(Error.NotFound("Member.NotFound", "Không tìm thấy hồ sơ hội viên của bạn."));
        }

        return Result.Success(memberDto);
    }
}
