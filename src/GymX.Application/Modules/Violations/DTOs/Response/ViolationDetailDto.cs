namespace GymX.Application.Modules.Violations.DTOs.Response;

public record ViolationDetailDto(
    Guid Id,
    string ViolationCode,
    Guid EmployeeId,
    string EmployeeCode,
    string EmployeeName,
    string EmployeePosition,
    Guid PenaltyRuleId,
    string RuleCode,
    string RuleName,
    string ViolationType,
    decimal FineAmount,
    DateTime OccurredAt,
    string? EvidenceUrl,
    string? Note,
    string Status,
    string? CancellationReason,
    DateTime? CancelledAt,
    string? CancelledByName,
    Guid RecordedBy,
    string RecordedByName,
    Guid? AttendanceRecordId,
    DateTime CreatedDate);

