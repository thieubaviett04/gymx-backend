using GymX.Domain.Entities.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");
        
        builder.HasIndex(e => e.UserId).IsUnique();
        builder.HasIndex(e => e.EmployeeCode).IsUnique();
        builder.HasIndex(e => e.NationalId).IsUnique();
        builder.HasIndex(e => e.Position);

        builder.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(20);
        builder.Property(e => e.NationalId).HasMaxLength(20);
        builder.Property(e => e.Position).IsRequired().HasMaxLength(30);
        builder.Property(e => e.EmploymentStatus).IsRequired().HasMaxLength(20);
        builder.Property(e => e.BaseSalaryRate).HasColumnType("numeric(12,2)");

        builder.HasOne<GymX.Domain.Entities.Identity.User>()
            .WithOne()
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.ToTable("specializations");
        
        builder.HasIndex(s => s.Name).IsUnique();

        builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Status).IsRequired().HasMaxLength(20);
    }
}

public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.ToTable("trainers");
        
        builder.HasIndex(t => t.EmployeeId).IsUnique();
        
        builder.Property(t => t.SalaryCoefficient).HasColumnType("numeric(5,2)");
        builder.Property(t => t.RatingAvg).HasColumnType("numeric(3,2)");

        builder.HasOne<Employee>()
            .WithOne()
            .HasForeignKey<Trainer>(t => t.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TrainerDocumentConfiguration : IEntityTypeConfiguration<TrainerDocument>
{
    public void Configure(EntityTypeBuilder<TrainerDocument> builder)
    {
        builder.ToTable("trainer_documents");

        builder.HasIndex(d => d.VerificationStatus);
        builder.HasIndex(d => d.ExpiryDate);

        builder.Property(d => d.DocumentType).IsRequired().HasMaxLength(50);
        builder.Property(d => d.DocumentName).IsRequired().HasMaxLength(150);
        builder.Property(d => d.IssuingOrganization).HasMaxLength(150);
        builder.Property(d => d.DocumentNumber).HasMaxLength(100);
        builder.Property(d => d.VerificationStatus).IsRequired().HasMaxLength(20);
    }
}

public class TrainingServiceConfiguration : IEntityTypeConfiguration<TrainingService>
{
    public void Configure(EntityTypeBuilder<TrainingService> builder)
    {
        builder.ToTable("training_services");

        builder.HasIndex(s => new { s.SpecializationId, s.Name }).IsUnique();

        builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Status).IsRequired().HasMaxLength(20);
        builder.Property(s => s.Price).HasColumnType("numeric(12,2)");
    }
}
