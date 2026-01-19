using TodoApp.Domain.Entities;

namespace TodoApp.Application.Service
{
    /// <summary>
    /// Service: Gửi realtime notifications đến admin dashboard qua SignalR
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Tạo và push notification về đơn hàng đến admin
        /// </summary>
        Task SendOrderNotificationAsync(string title, string message, object? metadata = null);

        /// <summary>
        /// Tạo và push notification về user đến admin
        /// </summary>
        Task SendUserNotificationAsync(string title, string message, object? metadata = null);

        /// <summary>
        /// Tạo và push notification hệ thống đến admin
        /// </summary>
        Task SendSystemNotificationAsync(string title, string message, object? metadata = null);

        /// <summary>
        /// Tạo và push notification generic
        /// </summary>
        Task SendNotificationAsync(NotificationType type, string title, string message, object? metadata = null);
    }
}
