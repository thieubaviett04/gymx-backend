using GymX.Domain.Common.Models;

namespace GymX.Domain.Entities.Notifications;

public class Notification : EntityAuditBase
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string NotificationType { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    protected Notification() { }

    public static Notification Create(Guid userId, string title, string content, string notificationType)
    {
        return new Notification
        {
            UserId = userId,
            Title = title,
            Content = content,
            NotificationType = notificationType,
            IsRead = false
        };
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}
