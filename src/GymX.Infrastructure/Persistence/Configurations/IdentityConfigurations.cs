using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymX.Domain.Entities.Identity;


namespace GymX.Infrastructure.Persistence.Configurations
{
    // Cấu hình EF Core cho bang User
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            
            builder.HasIndex(u => u.Email).HasDatabaseName("ix_users_email").IsUnique();
            builder.HasIndex(u => u.PhoneNumber).HasDatabaseName("ix_users_phone_number").IsUnique();
            builder.HasIndex(u => u.GoogleId).HasDatabaseName("ix_users_google_id").IsUnique();

            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.PasswordHash).HasMaxLength(255);
            builder.Property(u => u.GoogleId).HasMaxLength(255);
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Gender).HasMaxLength(10);
            builder.Property(u => u.PhoneNumber).HasMaxLength(20);
            builder.Property(u => u.Address).HasMaxLength(255);
            builder.Property(u => u.Status).IsRequired().HasMaxLength(30).HasColumnName("account_status");
        }
    }

    // Cấu hình EF Core cho bang Role
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.HasIndex(r => r.Code).IsUnique();
            builder.Property(r => r.Code).IsRequired().HasMaxLength(30);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        }
    }

    // Cấu hình EF Core cho bang UserRole
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("user_roles");
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });
            builder.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
            builder.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);
        }
    }

    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");
            builder.HasIndex(rt => rt.TokenHash).IsUnique();
            builder.Property(rt => rt.TokenHash).IsRequired().HasMaxLength(255);
            builder.HasOne(rt => rt.User).WithMany(u => u.RefreshTokens).HasForeignKey(rt => rt.UserId);
        }
    }

    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ToTable("password_reset_tokens");
            builder.Property(prt => prt.TokenHash).IsRequired().HasMaxLength(255);
            builder.HasOne(prt => prt.User).WithMany(u => u.PasswordResetTokens).HasForeignKey(prt => prt.UserId);
        }
    }
    public class FaceDataConfiguration : IEntityTypeConfiguration<FaceData>
    {
        public void Configure(EntityTypeBuilder<FaceData> builder)
        {
            builder.ToTable("face_data");

            builder.HasIndex(f => f.UserId).IsUnique();
            builder.HasIndex(f => new { f.Provider, f.ExternalFaceId }).IsUnique();

            builder.Property(f => f.Provider).IsRequired().HasMaxLength(50);
            builder.Property(f => f.ExternalFaceId).HasMaxLength(255);
            builder.Property(f => f.Status).IsRequired().HasMaxLength(20);

            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<FaceData>(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.EnrolledBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}