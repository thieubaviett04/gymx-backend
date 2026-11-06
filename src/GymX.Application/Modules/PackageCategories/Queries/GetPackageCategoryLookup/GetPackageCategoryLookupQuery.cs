using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.PackageCategories.Queries.GetPackageCategoryLookup;

public record GetPackageCategoryLookupQuery : IRequest<Result<List<PackageCategoryLookupDto>>>;

