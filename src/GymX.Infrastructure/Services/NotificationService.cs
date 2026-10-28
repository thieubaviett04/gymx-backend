using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Notifications.DTOs;
using GymX.Domain.Entities.Notifications;
using GymX.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GymX.Infrastructure.Services;

public class NotificationService(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    IHubContext<NotificationHub> hubContext)
    : INotificationService
{
    public async Task<NotificationDto> SendNotificationAsync(
        Guid userId,
        string title,
        string content,
        string notificationType,
        CancellationToken cancellationToken = default)
    {
        var notification = Notification.Create(userId, title, content, notificationType);

        await notificationRepository.AddAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Content = notification.Content,
            NotificationType = notification.NotificationType,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedDate = notification.CreatedDate
        };

        await hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", dto, cancellationToken);

        return dto;
    }

    public async Task<IReadOnlyList<NotificationDto>> SendBatchNotificationsAsync(
        IEnumerable<Guid> userIds,
        string title,
        string content,
        string notificationType,
        CancellationToken cancellationToken = default)
    {
        var idList = userIds.Distinct().ToList();
        if (idList.Count == 0)
        {
            return [];
        }

        var notifications = idList.Select(uid => Notification.Create(uid, title, content, notificationType)).ToList();

        await notificationRepository.AddRangeAsync(notifications, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            Content = n.Content,
            NotificationType = n.NotificationType,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedDate = n.CreatedDate
        }).ToList();

        foreach (var dto in dtos)
        {
            await hubContext.Clients.User(dto.UserId.ToString()).SendAsync("ReceiveNotification", dto, cancellationToken);
        }

        return dtos;
    }
}
