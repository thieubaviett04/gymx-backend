using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Attendance;

public class EmployeeWorkSchedule : EntityAuditBase
{
    public Guid EmployeeId { get; private set; }
    public Guid ShiftId { get; private set; }
    public DateOnly WorkDate { get; private set; }
    public string Status { get; private set; } = ScheduleStatus.Registered;
    public DateTime RegisteredAt { get; private set; }
    public Guid? ApprovedBy { get; private set; }

    protected EmployeeWorkSchedule() { }

    public static EmployeeWorkSchedule Create(Guid employeeId, Guid shiftId, DateOnly workDate)
    {
        return new EmployeeWorkSchedule
        {
            EmployeeId = employeeId,
            ShiftId = shiftId,
            WorkDate = workDate,
            Status = ScheduleStatus.Registered,
            RegisteredAt = DateTime.UtcNow
        };
    }

    public void Approve(Guid approvedBy)
    {
        Status = ScheduleStatus.Approved;
        ApprovedBy = approvedBy;
    }

    public void Cancel()
    {
        Status = ScheduleStatus.Cancelled;
    }
}
