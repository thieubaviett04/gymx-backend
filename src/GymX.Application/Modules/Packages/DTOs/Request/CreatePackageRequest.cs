namespace GymX.Application.Modules.Packages.DTOs.Request;

public class CreatePackageRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Status { get; set; }
    public int DurationMonths { get; set; }
    public string? Description { get; set; }
    public Guid? CategoryId { get; set; }
    public DateOnly EffectiveStartDate { get; set; }
}
