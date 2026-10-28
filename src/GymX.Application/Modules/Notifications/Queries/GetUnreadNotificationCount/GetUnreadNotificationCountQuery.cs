using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Notifications.Queries.GetUnreadNotificationCount;

public record GetUnreadNotificationCountQuery() : IRequest<Result<int>>;
