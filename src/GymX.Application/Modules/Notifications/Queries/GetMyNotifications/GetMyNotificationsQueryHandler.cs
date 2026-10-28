using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Application.Modules.Notifications.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandler(
    ICurrentUserService currentUserService,
    INotificationRepository notificationRepository)
    : IRequestHandler<GetMyNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    public async Task<Result<PagedResult<NotificationDto>>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentUserService.UserId) || !Guid.TryParse(currentUserService.UserId, out var userId))
        {
            return Result.Failure<PagedResult<NotificationDto>>(
                Error.Unauthorized("Auth.Unauthorized", "Vui lòng đăng nhập để thực hiện thao tác này."));
        }

        var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
        var pageSize = request.PageSize <= 0 ? 20 : (request.PageSize > 100 ? 100 : request.PageSize);

        var pagedNotifications = await notificationRepository.GetByUserIdAsync(
            userId,
            request.IsRead,
            request.NotificationType,
            pageIndex,
            pageSize,
            cancellationToken);

        var dtos = pagedNotifications.Items.Select(n => new NotificationDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            Content = n.Content,
            NotificationType = n.NotificationType,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedDate = n.CreatedDate
        }).ToList();

        var result = new PagedResult<NotificationDto>(
            dtos,
            pagedNotifications.TotalCount,
            pagedNotifications.PageIndex,
            pagedNotifications.PageSize);

        return Result.Success(result);
    }
}
