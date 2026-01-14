using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Service;

namespace TodoApp.Infrastructure.Services
{
    /// <summary>
    /// Implementation của INotificationService sử dụng SignalR
    /// Sử dụng IHubContext generic để tránh dependency vào WebAPI layer
    /// </summary>
    public class SignalRNotificationService<THub> : INotificationService where THub : Hub
    {
        private readonly IHubContext<THub> _hubContext;
        private readonly ILogger<SignalRNotificationService<THub>> _logger;

        public SignalRNotificationService(
            IHubContext<THub> hubContext,
            ILogger<SignalRNotificationService<THub>> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendOrderNotificationAsync(object notificationData)
        {
            try
            {
                // Gửi notification đến group "Admins"
                await _hubContext.Clients.Group("Admins")
                    .SendAsync("NewOrderNotification", notificationData);

                _logger.LogInformation("✅ SignalR notification sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send SignalR notification");
            }
        }
    }
}
