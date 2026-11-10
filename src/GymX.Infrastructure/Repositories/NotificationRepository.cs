using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Notifications.DTOs;
using GymX.Domain.Common.Models;
using GymX.Domain.Entities.Notifications;
using GymX.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GymX.Infrastructure.Repositories;

public class NotificationRepository(ApplicationDbContext context)
    : GenericRepository<Notification>(context), INotificationRepository
{
    public async Task<PagedResult<NotificationDto>> GetByUserIdAsync(
        Guid userId,
        bool? isRead,
        string? notificationType,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(n => n.UserId == userId);

        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value);
        }

        if (!string.IsNullOrWhiteSpace(notificationType))
        {
            query = query.Where(n => n.NotificationType == notificationType.Trim().ToUpper());
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(n => n.CreatedDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Content = n.Content,
                NotificationType = n.NotificationType,
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                CreatedDate = n.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<NotificationDto>(items, totalCount, pageIndex, pageSize);
    }

    public async Task<int> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
    }

    public async Task<int> MarkAllAsReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, DateTime.UtcNow),
                cancellationToken);
    }
}
