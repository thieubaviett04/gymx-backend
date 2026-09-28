using GymX.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.OrderCode).IsUnique();

        builder.Property(x => x.Amount)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(255);

        builder.Property(x => x.PaymentLinkId)
               .HasMaxLength(100);

        builder.Property(x => x.CheckoutUrl)
               .HasMaxLength(2000);
    }
}
