namespace GymX.Application.Modules.Packages.DTOs.Response;

public class PackageSummaryDto
{
    public Guid Id { get; set; }
    public string PackageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationMonths { get; set; }
    public int DurationDays { get; set; }
    public int? MaxCheckins { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateOnly EffectiveStartDate { get; set; }
    public DateOnly? EffectiveEndDate { get; set; }
    public Guid? PreviousPackageId { get; set; }
    public string? PreviousPackageCode { get; set; }
    public int ActiveMembersCount { get; set; }
    public DateTime CreatedDate { get; set; }
}
