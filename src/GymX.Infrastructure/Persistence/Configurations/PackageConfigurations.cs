using GymX.Domain.Entities.Packages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("packages");

        builder.HasIndex(p => p.PackageCode)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(p => p.Status);

        builder.Property(p => p.PackageCode).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Price).HasColumnType("numeric(12,2)");
        builder.Property(p => p.Status).IsRequired().HasMaxLength(20);

        builder.HasOne(p => p.PreviousPackage)
            .WithMany()
            .HasForeignKey(p => p.PreviousPackageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
