namespace GymX.Application.Modules.PackageCategories.DTOs;

public class PackageCategoryLookupDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
