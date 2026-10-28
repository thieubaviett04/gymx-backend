using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Notifications;

namespace GymX.Application.Common.Interfaces.Repositories;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<PagedResult<Notification>> GetByUserIdAsync(
        Guid userId,
        bool? isRead,
        string? notificationType,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<int> MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
