using GymX.Application.Modules.Notifications.DTOs;

namespace GymX.Application.Common.Interfaces;

public interface INotificationService
{
    Task<NotificationDto> SendNotificationAsync(
        Guid userId,
        string title,
        string content,
        string notificationType,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationDto>> SendBatchNotificationsAsync(
        IEnumerable<Guid> userIds,
        string title,
        string content,
        string notificationType,
        CancellationToken cancellationToken = default);
}
