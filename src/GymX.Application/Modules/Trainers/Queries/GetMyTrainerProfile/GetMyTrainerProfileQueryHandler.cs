using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Trainers.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Trainers.Queries.GetMyTrainerProfile;

public class GetMyTrainerProfileQueryHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository)
    : IRequestHandler<GetMyTrainerProfileQuery, Result<TrainerProfileDto>>
{
    public async Task<Result<TrainerProfileDto>> Handle(GetMyTrainerProfileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<TrainerProfileDto>(Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var profile = await userRepository.GetFullProfileAsync(userId, cancellationToken);
        if (profile?.TrainerInfo is null)
        {
            return Result.Failure<TrainerProfileDto>(Error.NotFound("Trainer.NotFound", "Không tìm thấy hồ sơ huấn luyện viên của bạn."));
        }

        return Result.Success(profile.TrainerInfo);
    }
}
