using GymX.Application.Modules.PackageCategories.Commands.CreatePackageCategory;
using GymX.Application.Modules.PackageCategories.Commands.DeletePackageCategory;
using GymX.Application.Modules.PackageCategories.Commands.UpdatePackageCategory;
using GymX.Application.Modules.PackageCategories.DTOs;
using GymX.Application.Modules.PackageCategories.Queries.GetPackageCategories;
using GymX.Application.Modules.PackageCategories.Queries.GetPackageCategoryById;
using GymX.Application.Modules.PackageCategories.Queries.GetPackageCategoryLookup;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PackageCategoriesController;

[ApiController]
[Route("api/v1/admin/package-categories")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class PackageCategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetPackageCategoriesQuery(search, status, pageIndex, pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Ok(new { success = true, data = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("lookup")]
    [Authorize]
    public async Task<IActionResult> GetLookup(CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPackageCategoryLookupQuery(), cancellationToken);

        return result.IsSuccess
            ? Ok(new { success = true, data = result.Value })
            : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPackageCategoryByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code == "PackageCategory.NotFound"
            ? NotFound(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePackageCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new CreatePackageCategoryCommand(request.Code, request.Name, request.Description),
            cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "PackageCategory.CodeAlreadyExists" => Conflict(new { error = result.Error }),
            "PackageCategory.NameAlreadyExists" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePackageCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new UpdatePackageCategoryCommand(id, request.Name, request.Description, request.Status),
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "PackageCategory.NotFound" => NotFound(new { error = result.Error }),
            "PackageCategory.NameAlreadyExists" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeletePackageCategoryCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Xóa loại hình gói tập thành công." });
        }

        return result.Error.Code switch
        {
            "PackageCategory.NotFound" => NotFound(new { error = result.Error }),
            "PackageCategory.CannotDeleteInUse" => Conflict(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}

