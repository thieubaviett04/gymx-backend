using GymX.Application.Modules.Packages.DTOs.Request;
using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Packages.Commands.CreatePackage;

public record CreatePackageCommand(
    string Name,
    decimal? Price,
    int DurationMonths,
    DateOnly EffectiveStartDate,
    string? Description,
    Guid? CategoryId = null,
    string? Status = null) : IRequest<Result<PackageDetailDto>>
{
    public static CreatePackageCommand FromRequest(CreatePackageRequest req) => new(
        req.Name.Trim(),
        req.Price,
        req.DurationMonths,
        req.EffectiveStartDate,
        string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
        req.CategoryId,
        string.IsNullOrWhiteSpace(req.Status) ? GymX.Domain.Constants.PackageStatus.Active : req.Status.Trim().ToUpperInvariant());
}
