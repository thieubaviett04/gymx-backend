namespace GymX.Application.Modules.PenaltyRules.DTOs.Request;

public class UpdatePenaltyRuleRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal? FineAmount { get; set; }
    public string SeverityLevel { get; set; } = string.Empty;
    public DateTime? EffectiveFrom { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ViolationType { get; set; }
    public string? Mode { get; set; }
}
