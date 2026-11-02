namespace GymX.Application.Modules.PenaltyRules.DTOs.Response;

public class PenaltyRuleSummaryDto
{
    public Guid Id { get; set; }
    public string RuleCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal FineAmount { get; set; }
    public string SeverityLevel { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Description { get; set; }
    public int ViolationsCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
