using GymX.Application.Modules.Notifications.DTOs;
using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery(
    bool? IsRead = null,
    string? NotificationType = null,
    int PageIndex = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<NotificationDto>>>;
