namespace GymX.Application.Modules.Violations.DTOs.Response;

public record ViolationSummaryDto(
    Guid Id,
    string ViolationCode,
    Guid EmployeeId,
    string EmployeeCode,
    string EmployeeName,
    string EmployeePosition,
    Guid PenaltyRuleId,
    string RuleCode,
    string RuleName,
    decimal FineAmount,
    DateTime OccurredAt,
    string Status,
    string? EvidenceUrl,
    DateTime CreatedDate);

