using GymX.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => new { n.UserId, n.IsRead });

        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.NotificationType).IsRequired().HasMaxLength(30);
    }
}
