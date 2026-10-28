using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Users.Commands.UploadAvatar;

public class UploadAvatarCommandHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IFileStorageService fileStorageService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UploadAvatarCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<string>(Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<string>(Error.NotFound("User.NotFound", "Không tìm thấy thông tin người dùng."));
        }

        var newAvatarUrl = await fileStorageService.SaveFileAsync(
            request.FileStream,
            request.FileName,
            "avatars",
            cancellationToken
        );

        if (!string.IsNullOrEmpty(user.AvatarUrl) && user.AvatarUrl.StartsWith("/uploads/"))
        {
            await fileStorageService.DeleteFileAsync(user.AvatarUrl, cancellationToken);
        }

        user.UpdateAvatar(newAvatarUrl);
        userRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(newAvatarUrl);
    }
}
