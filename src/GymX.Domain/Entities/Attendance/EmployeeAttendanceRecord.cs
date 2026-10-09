using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Attendance;

public class EmployeeAttendanceRecord : EntityAuditBase
{
    public Guid WorkScheduleId { get; private set; }
    public DateTime? CheckInAt { get; private set; }
    public DateTime? CheckOutAt { get; private set; }
    public int LateMinutes { get; private set; }
    public int EarlyLeaveMinutes { get; private set; }
    public string? CheckInMethod { get; private set; }
    public string? CheckOutMethod { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? Note { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    protected EmployeeAttendanceRecord() { }

    public static EmployeeAttendanceRecord Create(Guid workScheduleId, string status, string? note = null)
    {
        return new EmployeeAttendanceRecord
        {
            WorkScheduleId = workScheduleId,
            Status = status,
            Note = note,
            LateMinutes = 0,
            EarlyLeaveMinutes = 0
        };
    }
    
    public void MarkCheckIn(DateTime checkInTime, string method, int lateMinutes = 0)
    {
        CheckInAt = checkInTime;
        CheckInMethod = method;
        LateMinutes = lateMinutes;
    }

    public void MarkCheckOut(DateTime checkOutTime, string method, int earlyLeaveMinutes = 0)
    {
        CheckOutAt = checkOutTime;
        CheckOutMethod = method;
        EarlyLeaveMinutes = earlyLeaveMinutes;
    }
}
