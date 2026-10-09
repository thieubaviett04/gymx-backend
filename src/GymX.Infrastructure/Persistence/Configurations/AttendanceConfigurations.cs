using GymX.Domain.Entities.Attendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymX.Infrastructure.Persistence.Configurations;

public class ShiftDefinitionConfiguration : IEntityTypeConfiguration<ShiftDefinition>
{
    public void Configure(EntityTypeBuilder<ShiftDefinition> builder)
    {
        builder.ToTable("shift_definitions");
        
        builder.HasIndex(s => s.Name).IsUnique();

        builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Status).IsRequired().HasMaxLength(20);
    }
}

public class EmployeeWorkScheduleConfiguration : IEntityTypeConfiguration<EmployeeWorkSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeWorkSchedule> builder)
    {
        builder.ToTable("employee_work_schedules");
        
        builder.HasIndex(s => new { s.EmployeeId, s.ShiftId, s.WorkDate }).IsUnique();
        builder.HasIndex(s => new { s.EmployeeId, s.WorkDate });

        builder.Property(s => s.Status).IsRequired().HasMaxLength(20);
    }
}

public class EmployeeAttendanceRecordConfiguration : IEntityTypeConfiguration<EmployeeAttendanceRecord>
{
    public void Configure(EntityTypeBuilder<EmployeeAttendanceRecord> builder)
    {
        builder.ToTable("employee_attendance_records");
        
        builder.HasIndex(a => a.WorkScheduleId).IsUnique();
        builder.HasIndex(a => a.Status);

        builder.Property(a => a.CheckInMethod).HasMaxLength(30);
        builder.Property(a => a.CheckOutMethod).HasMaxLength(30);
        builder.Property(a => a.Status).IsRequired().HasMaxLength(30);
    }
}
