namespace GymX.Application.Modules.Kpis.DTOs.Response;

public record KpiTemplateDto(
    Guid Id,
    string Name,
    string TargetRole,
    int? TargetPtSessions,
    decimal TargetRevenue,
    int? TargetNewMembers,
    decimal TargetBonusAmount,
    string? Description,
    string Status,
    DateTime CreatedDate,
    DateTime? LastModifiedDate);

public record KpiTemplateSummaryDto(
    Guid Id,
    string Name,
    string TargetRole,
    int? TargetPtSessions,
    decimal TargetRevenue,
    int? TargetNewMembers,
    decimal TargetBonusAmount,
    string? Description,
    string Status,
    DateTime CreatedDate);

public record KpiTemplateLookupDto(
    Guid Id,
    string Name,
    string TargetRole,
    int? TargetPtSessions,
    decimal TargetRevenue,
    int? TargetNewMembers,
    decimal TargetBonusAmount);

public record EmployeeKpiProgressSummaryDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    string FullName,
    string Position,
    string TemplateName,
    int Month,
    int Year,
    int? TargetPtSessions,
    int ActualPtSessions,
    decimal SessionProgress,
    decimal TargetRevenue,
    decimal ActualRevenue,
    decimal RevenueProgress,
    int? TargetNewMembers,
    int ActualNewMembers,
    decimal OverallProgress,
    decimal TargetBonusAmount,
    string Status,
    string? Note,
    DateTime AssignedAt);

public record EmployeeKpiDetailDto(
    Guid Id,
    Guid EmployeeId,
    string EmployeeCode,
    string FullName,
    string Position,
    Guid KpiTemplateId,
    string TemplateName,
    int Month,
    int Year,
    int? TargetPtSessions,
    int ActualPtSessions,
    decimal SessionProgress,
    decimal TargetRevenue,
    decimal ActualRevenue,
    decimal RevenueProgress,
    int? TargetNewMembers,
    int ActualNewMembers,
    decimal OverallProgress,
    decimal TargetBonusAmount,
    string Status,
    string? Note,
    DateTime AssignedAt);

public record AssignKpiResultDto(
    int AssignedCount,
    int SkippedCount,
    string Message);

public record MyKpiDto(
    Guid Id,
    string TemplateName,
    int Month,
    int Year,
    int? TargetPtSessions,
    int ActualPtSessions,
    decimal SessionProgress,
    decimal TargetRevenue,
    decimal ActualRevenue,
    decimal RevenueProgress,
    decimal TargetBonusAmount,
    decimal OverallProgress,
    string Status,
    string? Note);

