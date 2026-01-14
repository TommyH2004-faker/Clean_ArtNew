namespace TodoApp.Application.Service
{
    /// <summary>
    /// Service để gửi realtime notifications đến clients
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Gửi notification về đơn hàng mới đến admin
        /// </summary>
        Task SendOrderNotificationAsync(object notificationData);
    }
}
