namespace GymX.Application.Modules.Packages.DTOs.Request;

public class UpdatePackageRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int DurationMonths { get; set; }
    public int? MaxCheckins { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public DateOnly EffectiveStartDate { get; set; }
    public DateOnly? EffectiveEndDate { get; set; }
    public string? Mode { get; set; }
}
