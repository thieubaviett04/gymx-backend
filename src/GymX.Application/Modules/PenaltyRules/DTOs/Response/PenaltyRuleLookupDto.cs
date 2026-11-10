namespace GymX.Application.Modules.PenaltyRules.DTOs.Response;

public record PenaltyRuleLookupDto(
    Guid Id,
    string RuleCode,
    string Name,
    decimal FineAmount,
    string SeverityLevel,
    string TargetAudience,
    DateOnly EffectiveDate,
    string? Description);

