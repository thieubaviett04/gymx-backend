namespace GymX.Application.Modules.PackageCategories.DTOs;

public class UpdatePackageCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
}

