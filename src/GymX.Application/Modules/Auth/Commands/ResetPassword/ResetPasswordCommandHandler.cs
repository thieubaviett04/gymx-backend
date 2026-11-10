using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Auth.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace GymX.Application.Modules.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IMemoryCache memoryCache,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ResetPasswordCommand, Result<ResetPasswordResponse>>
{
    public async Task<Result<ResetPasswordResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var tokenCache = $"ResetToken_{request.Token}";
        if (!memoryCache.TryGetValue(tokenCache, out string? email) || string.IsNullOrEmpty(email))
        {
            return Result.Failure<ResetPasswordResponse>(
                Error.Validation("Auth.InvalidToken", "Phiên đặt lại mật khẩu đã hết hạn hoặc không hợp lệ."));
        }

        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return Result.Failure<ResetPasswordResponse>(
                Error.NotFound("Auth.UserNotFound", "Không tìm thấy tài khoản người dùng"));
        }

        var newPasswordHash = passwordHasher.Hash(request.NewPassword);
        user.UpdatePassword(newPasswordHash);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        memoryCache.Remove(tokenCache);

        return Result.Success(new ResetPasswordResponse
        {
            Email = user.Email,
            Message = "Đặt lại mật khẩu thành công"
        });
    }
}
