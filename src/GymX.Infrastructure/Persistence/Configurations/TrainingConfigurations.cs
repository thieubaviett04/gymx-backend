using GymX.Domain.Entities.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class TrainingSessionConfiguration : IEntityTypeConfiguration<TrainingSession>
{
    public void Configure(EntityTypeBuilder<TrainingSession> builder)
    {
        builder.ToTable("training_sessions");
        
        builder.HasIndex(s => new { s.TrainerId, s.StartAt });
        builder.HasIndex(s => new { s.MemberId, s.StartAt });

        builder.Property(s => s.Status).IsRequired().HasMaxLength(20);
        builder.Property(s => s.PriceAtBooking).HasColumnType("numeric(12,2)");
    }
}

public class TrainingProgressConfiguration : IEntityTypeConfiguration<TrainingProgress>
{
    public void Configure(EntityTypeBuilder<TrainingProgress> builder)
    {
        builder.ToTable("training_progress");
        
        builder.HasIndex(p => p.MemberId);
        builder.HasIndex(p => p.TrainerId);
        
        builder.Property(p => p.Weight).HasColumnType("numeric(5,2)");
        builder.Property(p => p.BodyFatPercentage).HasColumnType("numeric(5,2)");
    }
}

public class TrainerReviewConfiguration : IEntityTypeConfiguration<TrainerReview>
{
    public void Configure(EntityTypeBuilder<TrainerReview> builder)
    {
        builder.ToTable("trainer_reviews");
        
        builder.HasIndex(r => r.TrainingSessionId).IsUnique();

        builder.Property(r => r.Comment).HasMaxLength(500);
    }
}
