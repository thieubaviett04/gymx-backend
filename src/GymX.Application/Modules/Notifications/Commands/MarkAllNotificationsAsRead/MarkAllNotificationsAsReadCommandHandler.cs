using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Notifications.Commands.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadCommandHandler(
    ICurrentUserService currentUserService,
    INotificationRepository notificationRepository)
    : IRequestHandler<MarkAllNotificationsAsReadCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<int>(
                Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var updatedCount = await notificationRepository.MarkAllAsReadAsync(userId, cancellationToken);
        return Result.Success(updatedCount);
    }
}
