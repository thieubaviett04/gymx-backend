using GymX.Application.Common.Interfaces;
using GymX.Application.Modules.Notifications.Commands.DeleteNotification;
using GymX.Application.Modules.Notifications.Commands.MarkAllNotificationsAsRead;
using GymX.Application.Modules.Notifications.Commands.MarkNotificationAsRead;
using GymX.Application.Modules.Notifications.Queries.GetMyNotifications;
using GymX.Application.Modules.Notifications.Queries.GetUnreadNotificationCount;
using GymX.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymX.Api.Controllers.NotificationsController;

public record SendTestNotificationRequest(string Title, string Content, string? NotificationType);

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] bool? isRead,
        [FromQuery] string? type,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetMyNotificationsQuery(isRead, type, pageIndex, pageSize), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = result.Value });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetUnreadNotificationCountQuery(), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, data = new { count = result.Value } });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPost("test-send")]
    public async Task<IActionResult> SendTestNotification(
        [FromBody] SendTestNotificationRequest request,
        [FromServices] INotificationService notificationService,
        [FromServices] ICurrentUserService currentUserService,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Unauthorized(new { error = new { code = "Auth.Unauthorized", description = "Vui lòng đăng nhập để thực hiện thao tác này." } });
        }

        var type = string.IsNullOrWhiteSpace(request.NotificationType) ? NotificationType.System : request.NotificationType;
        var dto = await notificationService.SendNotificationAsync(userId, request.Title, request.Content, type, cancellationToken);

        return Ok(new { success = true, data = dto, message = "Đã gửi thông báo thử nghiệm thành công." });
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new MarkNotificationAsReadCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Đã đánh dấu thông báo là đã đọc." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "Notification.NotFound" => NotFound(new { error = result.Error }),
            "Notification.Forbidden" => StatusCode(StatusCodes.Status403Forbidden, new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new MarkAllNotificationsAsReadCommand(), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Đã đánh dấu tất cả thông báo là đã đọc.", data = new { updatedCount = result.Value } });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteNotification([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new DeleteNotificationCommand(id), cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = "Xoá thông báo thành công." });
        }

        return result.Error.Code switch
        {
            "Auth.Unauthorized" => Unauthorized(new { error = result.Error }),
            "Notification.NotFound" => NotFound(new { error = result.Error }),
            "Notification.Forbidden" => StatusCode(StatusCodes.Status403Forbidden, new { error = result.Error }),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
