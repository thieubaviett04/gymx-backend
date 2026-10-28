using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Users.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
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

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
        {
            var existingPhone = await userRepository.GetByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
            if (existingPhone is not null && existingPhone.Id != user.Id)
            {
                return Result.Failure<UserProfileDto>(Error.Conflict("User.PhoneExists", "Số điện thoại này đã được sử dụng bởi người khác."));
            }
        }

        user.UpdatePartialProfileInfo(
            request.FullName,
            request.PhoneNumber,
            request.Gender,
            request.DateOfBirth,
            request.Address
        );
        userRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProfile = await userRepository.GetFullProfileAsync(user.Id, cancellationToken);

        return Result.Success(updatedProfile!);
    }
}
