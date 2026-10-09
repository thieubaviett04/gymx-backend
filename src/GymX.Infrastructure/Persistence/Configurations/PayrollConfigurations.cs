using GymX.Domain.Entities.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class PenaltyRuleConfiguration : IEntityTypeConfiguration<PenaltyRule>
{
    public void Configure(EntityTypeBuilder<PenaltyRule> builder)
    {
        builder.ToTable("penalty_rules");

        builder.Property(r => r.Name).IsRequired().HasMaxLength(150);
        builder.Property(r => r.ViolationType).IsRequired().HasMaxLength(50);
        builder.Property(r => r.FineAmount).HasColumnType("numeric(12,2)");
        builder.Property(r => r.SeverityLevel).HasMaxLength(20);
        builder.Property(r => r.Status).IsRequired().HasMaxLength(20);
    }
}

public class ViolationConfiguration : IEntityTypeConfiguration<Violation>
{
    public void Configure(EntityTypeBuilder<Violation> builder)
    {
        builder.ToTable("violations");

        builder.HasIndex(v => v.EmployeeId);
        builder.HasIndex(v => v.PenaltyRuleId);
        builder.HasIndex(v => v.AttendanceRecordId);

        builder.Property(v => v.FineAmount).HasColumnType("numeric(12,2)");
    }
}

public class SalaryFormulaConfiguration : IEntityTypeConfiguration<SalaryFormula>
{
    public void Configure(EntityTypeBuilder<SalaryFormula> builder)
    {
        builder.ToTable("salary_formulas");

        builder.Property(f => f.Name).IsRequired().HasMaxLength(100);
        builder.Property(f => f.BaseRateType).IsRequired().HasMaxLength(20);
        builder.Property(f => f.BaseRate).HasColumnType("numeric(12,2)");
        builder.Property(f => f.Status).IsRequired().HasMaxLength(20);
        
        // Map CreatorId property in C# to created_by column in DB
        builder.Property(f => f.CreatorId).HasColumnName("created_by");
    }
}

public class PayrollPeriodConfiguration : IEntityTypeConfiguration<PayrollPeriod>
{
    public void Configure(EntityTypeBuilder<PayrollPeriod> builder)
    {
        builder.ToTable("payroll_periods");

        builder.HasIndex(p => new { p.PeriodStart, p.PeriodEnd }).IsUnique();
        builder.HasIndex(p => p.SalaryFormulaId);

        builder.Property(p => p.Status).IsRequired().HasMaxLength(20);
    }
}

public class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    public void Configure(EntityTypeBuilder<Payslip> builder)
    {
        builder.ToTable("payslips");

        builder.HasIndex(p => new { p.PayrollPeriodId, p.EmployeeId }).IsUnique();
        builder.HasIndex(p => p.EmployeeId);

        builder.Property(p => p.BaseSalary).HasColumnType("numeric(12,2)");
        builder.Property(p => p.BonusAmount).HasColumnType("numeric(12,2)");
        builder.Property(p => p.PenaltyAmount).HasColumnType("numeric(12,2)");
        builder.Property(p => p.TotalSalary).HasColumnType("numeric(12,2)");
        builder.Property(p => p.Status).IsRequired().HasMaxLength(20);
    }
}
