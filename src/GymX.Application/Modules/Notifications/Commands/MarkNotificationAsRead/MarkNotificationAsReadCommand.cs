using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Notifications.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest<Result<bool>>;
