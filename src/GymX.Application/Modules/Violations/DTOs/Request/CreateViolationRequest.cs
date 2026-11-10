namespace GymX.Application.Modules.Violations.DTOs.Request;

public record CreateViolationRequest(
    Guid EmployeeId,
    Guid PenaltyRuleId,
    DateTime OccurredAt,
    Guid? AttendanceRecordId = null,
    string? EvidenceUrl = null,
    string? Note = null);

