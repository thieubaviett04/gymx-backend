using GymX.Domain.Entities.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");
        
        builder.HasIndex(m => m.UserId).IsUnique();
        builder.HasIndex(m => m.MemberCode).IsUnique();

        builder.Property(m => m.MemberCode).IsRequired().HasMaxLength(20);
        builder.Property(m => m.EmergencyContactName).HasMaxLength(150);
        builder.Property(m => m.EmergencyContactPhone).HasMaxLength(20);
        builder.Property(m => m.MemberStatus).IsRequired().HasMaxLength(20);

        builder.HasOne<GymX.Domain.Entities.Identity.User>()
            .WithOne()
            .HasForeignKey<Member>(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class MemberPackageConfiguration : IEntityTypeConfiguration<MemberPackage>
{
    public void Configure(EntityTypeBuilder<MemberPackage> builder)
    {
        builder.ToTable("member_packages");
        
        builder.HasIndex(m => new { m.MemberId, m.Status });

        builder.Property(m => m.Status).IsRequired().HasMaxLength(30);
        builder.Property(m => m.PriceAtPurchase).HasColumnType("numeric(12,2)");
    }
}

public class DayTicketConfiguration : IEntityTypeConfiguration<DayTicket>
{
    public void Configure(EntityTypeBuilder<DayTicket> builder)
    {
        builder.ToTable("day_tickets");
        
        builder.HasIndex(t => t.MemberId);
        builder.HasIndex(t => t.SoldBy);
        builder.HasIndex(t => t.TicketDate);

        builder.Property(t => t.GuestName).HasMaxLength(150);
        builder.Property(t => t.GuestPhone).HasMaxLength(20);
        builder.Property(t => t.Status).IsRequired().HasMaxLength(20);
        builder.Property(t => t.PriceAtPurchase).HasColumnType("numeric(12,2)");
    }
}

public class MemberCheckinConfiguration : IEntityTypeConfiguration<MemberCheckin>
{
    public void Configure(EntityTypeBuilder<MemberCheckin> builder)
    {
        builder.ToTable("member_checkins");
        
        builder.HasIndex(c => new { c.MemberId, c.CheckInAt });

        builder.Property(c => c.Method).IsRequired().HasMaxLength(30);
        builder.Property(c => c.Status).IsRequired().HasMaxLength(30);
    }
}
