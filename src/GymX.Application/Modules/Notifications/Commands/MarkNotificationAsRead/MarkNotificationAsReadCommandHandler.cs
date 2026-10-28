using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadCommandHandler(
    ICurrentUserService currentUserService,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<MarkNotificationAsReadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<bool>(
                Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var notification = await notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification is null)
        {
            return Result.Failure<bool>(
                Error.NotFound("Notification.NotFound", "Không tìm thấy thông báo."));
        }

        if (notification.UserId != userId)
        {
            return Result.Failure<bool>(
                Error.Forbidden("Notification.Forbidden", "Bạn không có quyền thao tác trên thông báo này."));
        }

        if (!notification.IsRead)
        {
            notification.MarkAsRead();
            notificationRepository.Update(notification);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(true);
    }
}
