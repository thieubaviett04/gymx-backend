using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Payroll;

public class Violation : EntityAuditBase
{
    public Guid EmployeeId { get; private set; }
    public Guid PenaltyRuleId { get; private set; }
    public Guid? AttendanceRecordId { get; private set; }
    public decimal FineAmount { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public string? Note { get; private set; }
    public Guid RecordedBy { get; private set; }

    protected Violation() { }

    public static Violation Create(Guid employeeId, Guid penaltyRuleId, decimal fineAmount, DateTime occurredAt, Guid recordedBy, Guid? attendanceRecordId = null, string? note = null)
    {
        return new Violation
        {
            EmployeeId = employeeId,
            PenaltyRuleId = penaltyRuleId,
            FineAmount = fineAmount,
            OccurredAt = occurredAt,
            RecordedBy = recordedBy,
            AttendanceRecordId = attendanceRecordId,
            Note = note
        };
    }
}
