using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Notifications.Commands.MarkAllNotificationsAsRead;

public record MarkAllNotificationsAsReadCommand() : IRequest<Result<int>>;
