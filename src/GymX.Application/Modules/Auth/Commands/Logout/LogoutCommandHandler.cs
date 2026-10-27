using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Auth.Commands.Logout;

public class LogoutCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<LogoutCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (storedToken is not null && storedToken.IsActive)
        {
            storedToken.Revoke();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success("Đăng xuất thành công.");
    }
}
