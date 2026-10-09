using GymX.Domain.Entities.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.HasIndex(i => i.MemberId);
        builder.HasIndex(i => new { i.ReferenceType, i.ReferenceId });
        builder.HasIndex(i => i.Status);

        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(30);
        builder.Property(i => i.CustomerName).HasMaxLength(150);
        builder.Property(i => i.CustomerPhone).HasMaxLength(20);
        builder.Property(i => i.ReferenceType).IsRequired().HasMaxLength(30);
        builder.Property(i => i.Subtotal).HasColumnType("numeric(12,2)");
        builder.Property(i => i.TotalAmount).HasColumnType("numeric(12,2)");
        builder.Property(i => i.Status).IsRequired().HasMaxLength(20);
        
        // Map CreatorId property in C# to created_by column in DB
        builder.Property(i => i.CreatorId).HasColumnName("created_by");

        builder.HasOne<GymX.Domain.Entities.Identity.User>()
            .WithMany()
            .HasForeignKey(i => i.CreatorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasIndex(p => p.InvoiceId);
        builder.HasIndex(p => p.GatewayTransactionRef);

        builder.Property(p => p.Method).IsRequired().HasMaxLength(30);
        builder.Property(p => p.Amount).HasColumnType("numeric(12,2)");
        builder.Property(p => p.GatewayTransactionRef).HasMaxLength(100);
        builder.Property(p => p.Status).IsRequired().HasMaxLength(20);
    }
}
