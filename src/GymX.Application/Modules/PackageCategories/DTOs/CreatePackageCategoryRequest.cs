namespace GymX.Application.Modules.PackageCategories.DTOs;

public class CreatePackageCategoryRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
