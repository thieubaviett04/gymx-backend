using GymX.Domain.Common.Models;
using GymX.Domain.Constants.Payroll;
using GymX.Domain.Entities.Attendance;
using GymX.Domain.Entities.Employees;

namespace GymX.Domain.Entities.Payroll;

public class Violation : EntityAuditBase
{
    public string ViolationCode { get; private set; } = string.Empty;
    public Guid EmployeeId { get; private set; }
    public Guid PenaltyRuleId { get; private set; }
    public Guid? AttendanceRecordId { get; private set; }
    public decimal FineAmount { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public string? EvidenceUrl { get; private set; }
    public string? Note { get; private set; }
    public string Status { get; private set; } = ViolationStatus.Confirmed;
    public string? CancellationReason { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public Guid? CancelledBy { get; private set; }
    public Guid RecordedBy { get; private set; }

    public virtual Employee Employee { get; private set; } = null!;
    public virtual PenaltyRule PenaltyRule { get; private set; } = null!;
    public virtual EmployeeAttendanceRecord? AttendanceRecord { get; private set; }

    protected Violation() { }

    public static Violation Create(
        string violationCode,
        Guid employeeId,
        Guid penaltyRuleId,
        decimal fineAmount,
        DateTime occurredAt,
        Guid recordedBy,
        Guid? attendanceRecordId = null,
        string? evidenceUrl = null,
        string? note = null)
    {
        var normalizedOccurredAt = occurredAt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(occurredAt, DateTimeKind.Utc)
            : occurredAt.ToUniversalTime();

        return new Violation
        {
            ViolationCode = violationCode.Trim().ToUpperInvariant(),
            EmployeeId = employeeId,
            PenaltyRuleId = penaltyRuleId,
            FineAmount = fineAmount,
            OccurredAt = normalizedOccurredAt,
            RecordedBy = recordedBy,
            AttendanceRecordId = attendanceRecordId,
            EvidenceUrl = evidenceUrl?.Trim(),
            Note = note?.Trim(),
            Status = ViolationStatus.Confirmed
        };
    }

    public void Cancel(string reason, Guid cancelledBy)
    {
        Status = ViolationStatus.Cancelled;
        CancellationReason = reason.Trim();
        CancelledBy = cancelledBy;
        CancelledAt = DateTime.UtcNow;
    }

    public void RegenerateCode(string newCode)
    {
        ViolationCode = newCode.Trim().ToUpperInvariant();
    }
}
