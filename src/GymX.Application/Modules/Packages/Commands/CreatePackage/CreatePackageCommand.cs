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
    DateOnly? EffectiveEndDate,
    int? MaxCheckins,
    string? Description) : IRequest<Result<PackageDetailDto>>
{
    public static CreatePackageCommand FromRequest(CreatePackageRequest req) => new(
        req.Name.Trim(),
        req.Price,
        req.DurationMonths,
        req.EffectiveStartDate,
        req.EffectiveEndDate,
        req.MaxCheckins,
        string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim());
}
