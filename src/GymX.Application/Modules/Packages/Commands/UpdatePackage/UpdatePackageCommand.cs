using GymX.Application.Modules.Packages.DTOs.Request;
using GymX.Application.Modules.Packages.DTOs.Response;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Packages.Commands.UpdatePackage;

public record UpdatePackageCommand(
    Guid PackageId,
    string Name,
    decimal? Price,
    int DurationMonths,
    DateOnly EffectiveStartDate,
    DateOnly? EffectiveEndDate,
    string? Status,
    int? MaxCheckins,
    string? Description,
    string? Mode = null) : IRequest<Result<PackageDetailDto>>
{
    public static UpdatePackageCommand FromRequest(Guid packageId, UpdatePackageRequest req) => new(
        packageId,
        req.Name.Trim(),
        req.Price,
        req.DurationMonths,
        req.EffectiveStartDate,
        req.EffectiveEndDate,
        req.Status?.Trim().ToUpperInvariant(),
        req.MaxCheckins,
        string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
        req.Mode?.Trim().ToUpperInvariant());
}
