using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler(
    ICurrentUserService currentUserService,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangePasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
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

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return Result.Failure<string>(Error.Validation(
                "Auth.GoogleAccount",
                "Tài khoản của bạn được liên kết qua Google, không sử dụng mật khẩu độc lập."));
        }

        var isOldPasswordValid = passwordHasher.Verify(request.CurrentPassword, user.PasswordHash);
        if (!isOldPasswordValid)
        {
            return Result.Failure<string>(Error.Validation("Auth.WrongPassword", "Mật khẩu hiện tại không chính xác."));
        }

        var newHash = passwordHasher.Hash(request.NewPassword);
        user.UpdatePassword(newHash);
        userRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Đổi mật khẩu thành công.");
    }
}
