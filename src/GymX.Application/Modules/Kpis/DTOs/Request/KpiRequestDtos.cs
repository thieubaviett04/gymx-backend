namespace GymX.Application.Modules.Kpis.DTOs.Request;

public class CreateKpiTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public decimal TargetRevenue { get; set; }
    public decimal TargetBonusAmount { get; set; }
    public int? TargetPtSessions { get; set; }
    public int? TargetNewMembers { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
}

public class UpdateKpiTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public decimal TargetRevenue { get; set; }
    public decimal TargetBonusAmount { get; set; }
    public int? TargetPtSessions { get; set; }
    public int? TargetNewMembers { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class AssignEmployeeKpiRequest
{
    public Guid KpiTemplateId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public List<Guid> EmployeeIds { get; set; } = [];
    public string? Note { get; set; }
}
