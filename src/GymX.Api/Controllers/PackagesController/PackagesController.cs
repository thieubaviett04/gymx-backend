using GymX.Application.Modules.Packages.Commands.CreatePackage;
using GymX.Application.Modules.Packages.Commands.DeletePackage;
using GymX.Application.Modules.Packages.Commands.DiscontinuePackage;
using GymX.Application.Modules.Packages.Commands.UpdatePackage;
using GymX.Application.Modules.Packages.DTOs.Request;
using GymX.Application.Modules.Packages.Queries.GetPackageById;
using GymX.Application.Modules.Packages.Queries.GetPackages;
using GymX.Domain.Constants;
using GymX.Domain.Constants.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.PackagesController;

[ApiController]
[Route("api/v1/admin/packages")]
[Authorize(Policy = PolicyNames.RequireAdmin)]
public class PackagesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPackages(
        [FromQuery] string? search,
        [FromQuery] string? durationFilter,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetPackagesQuery(search, durationFilter, status, sortBy, pageIndex, pageSize),
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPackageById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetPackageByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "Package.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPost]
    public async Task<IActionResult> CreatePackage([FromBody] CreatePackageRequest request, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(CreatePackageCommand.FromRequest(request), cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(
                nameof(GetPackageById),
                new { id = result.Value.Id },
                new { success = true, data = result.Value, message = "Thêm gói tập thành công." });
        }

        return BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePackage(
        [FromRoute] Guid id,
        [FromBody] UpdatePackageRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(UpdatePackageCommand.FromRequest(id, request), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value, message = "Cập nhật gói tập thành công." });
        }

        return result.Error.Code switch
        {
            "Package.NotFound" => NotFound(new { error = result.Error }),
            "Package.InUse" => Conflict(new { error = result.Error, canDiscontinueAndCreate = true }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePackage([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeletePackageCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Xóa gói tập thành công." });
        }

        return result.Error.Code switch
        {
            "Package.NotFound" => NotFound(new { error = result.Error }),
            "Package.CannotDeleteInUse" => Conflict(new { error = result.Error, canDiscontinue = true }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPatch("{id:guid}/discontinue")]
    public async Task<IActionResult> DiscontinuePackage([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DiscontinuePackageCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Đã cập nhật trạng thái Ngừng kinh doanh cho gói tập." });
        }

        return result.Error.Code switch
        {
            "Package.NotFound" => NotFound(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpGet("/api/v1/packages")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicPackages(
        [FromQuery] string? search,
        [FromQuery] string? durationFilter,
        [FromQuery] string? sortBy,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetPackagesQuery(search, durationFilter, PackageStatus.Active, sortBy, pageIndex, pageSize),
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return BadRequest(new { error = result.Error });
    }
}
