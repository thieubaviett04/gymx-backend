using GymX.Domain.Entities.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class KpiTemplateConfiguration : IEntityTypeConfiguration<KpiTemplate>
{
    public void Configure(EntityTypeBuilder<KpiTemplate> builder)
    {
        builder.ToTable("kpi_templates");

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.TargetRole)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(t => t.TargetRevenue)
            .HasColumnType("numeric(14,2)");

        builder.Property(t => t.TargetBonusAmount)
            .HasColumnType("numeric(14,2)");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.TargetRole);
        builder.HasIndex(t => t.Status);
    }
}

public class EmployeeKpiConfiguration : IEntityTypeConfiguration<EmployeeKpi>
{
    public void Configure(EntityTypeBuilder<EmployeeKpi> builder)
    {
        builder.ToTable("employee_kpis");

        builder.Property(k => k.TargetRevenue)
            .HasColumnType("numeric(14,2)");

        builder.Property(k => k.TargetBonusAmount)
            .HasColumnType("numeric(14,2)");

        builder.Property(k => k.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(k => k.Note)
            .HasMaxLength(1000);

        builder.HasIndex(k => new { k.EmployeeId, k.Month, k.Year })
            .IsUnique();

        builder.HasIndex(k => new { k.Month, k.Year });
        builder.HasIndex(k => k.Status);

        builder.HasOne(k => k.Employee)
            .WithMany()
            .HasForeignKey(k => k.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(k => k.KpiTemplate)
            .WithMany()
            .HasForeignKey(k => k.KpiTemplateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

