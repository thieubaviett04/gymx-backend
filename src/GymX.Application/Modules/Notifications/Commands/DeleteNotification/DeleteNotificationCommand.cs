using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Notifications.Commands.DeleteNotification;

public record DeleteNotificationCommand(Guid NotificationId) : IRequest<Result<bool>>;
