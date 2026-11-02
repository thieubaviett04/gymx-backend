namespace GymX.Application.Modules.PenaltyRules.DTOs.Request;

public class CreatePenaltyRuleRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal? FineAmount { get; set; }
    public string SeverityLevel { get; set; } = string.Empty;
    public DateTime? EffectiveFrom { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
}
