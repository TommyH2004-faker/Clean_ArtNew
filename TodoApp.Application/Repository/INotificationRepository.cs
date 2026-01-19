using TodoApp.Domain.Entities;

namespace TodoApp.Application.Repository
{
    /// <summary>
    /// Repository interface: Quản lý Notifications
    /// </summary>
    public interface INotificationRepository
    {
        /// <summary>
        /// Thêm notification mới
        /// </summary>
        Task<Notification> AddAsync(Notification notification);

        /// <summary>
        /// Lấy tất cả notifications (admin dashboard)
        /// </summary>
        Task<List<Notification>> GetAllAsync(int page = 1, int pageSize = 50);

        /// <summary>
        /// Lấy notifications chưa đọc
        /// </summary>
        Task<List<Notification>> GetUnreadAsync();

        /// <summary>
        /// Đếm số notifications chưa đọc
        /// </summary>
        Task<int> CountUnreadAsync();

        /// <summary>
        /// Lấy notification theo ID
        /// </summary>
        Task<Notification?> GetByIdAsync(int idNotification);

        /// <summary>
        /// Cập nhật notification
        /// </summary>
        Task UpdateAsync(Notification notification);

        /// <summary>
        /// Đánh dấu tất cả đã đọc
        /// </summary>
        Task MarkAllAsReadAsync();
    }
}
