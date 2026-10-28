using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GymX.Infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}
