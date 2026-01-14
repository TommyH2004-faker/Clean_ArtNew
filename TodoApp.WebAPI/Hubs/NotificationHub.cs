using Microsoft.AspNetCore.SignalR;

namespace TodoApp.WebAPI.Hubs
{
    /// <summary>
    /// SignalR Hub cho realtime notifications
    /// </summary>
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Khi admin/user connect, có thể join group theo role
            var httpContext = Context.GetHttpContext();
            var userRole = httpContext?.User?.FindFirst("role")?.Value;

            if (userRole == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
