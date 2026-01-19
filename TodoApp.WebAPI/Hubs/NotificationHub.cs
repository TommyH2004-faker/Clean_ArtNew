using Microsoft.AspNetCore.SignalR;

namespace TodoApp.WebAPI.Hubs
{
    /// <summary>
    /// SignalR Hub: Gửi thông báo realtime đến admin dashboard
    /// </summary>
    public class NotificationHub : Hub
    {
        /// <summary>
        /// Được gọi khi client kết nối
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[SIGNALR] Admin connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Được gọi khi client ngắt kết nối
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[SIGNALR] Admin disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Method từ client: Đánh dấu notification đã đọc
        /// </summary>
        public async Task MarkNotificationAsRead(int notificationId)
        {
            // Admin sẽ gọi method này từ frontend
            Console.WriteLine($"[SIGNALR] Notification {notificationId} marked as read by {Context.ConnectionId}");
            await Task.CompletedTask;
        }
    }
}
