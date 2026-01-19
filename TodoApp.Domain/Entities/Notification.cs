namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity: Thông báo cho admin dashboard
    /// </summary>
    public class Notification
    {
        public int IdNotification { get; set; }
        
        /// <summary>
        /// Loại thông báo: Order, User, System
        /// </summary>
        public NotificationType Type { get; private set; }
        
        /// <summary>
        /// Tiêu đề thông báo
        /// </summary>
        public required string Title { get; set; }
        
        /// <summary>
        /// Nội dung chi tiết
        /// </summary>
        public required string Message { get; set; }
        
        /// <summary>
        /// Metadata dạng JSON (chứa thông tin chi tiết đơn hàng, user, etc.)
        /// </summary>
        public string? MetadataJson { get; private set; }
        
        /// <summary>
        /// Đã đọc chưa
        /// </summary>
        public bool IsRead { get; private set; }
        
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Thời gian đọc (nếu đã đọc)
        /// </summary>
        public DateTime? ReadAt { get; private set; }

        // EF Core constructor
        private Notification() { }

        /// <summary>
        /// Factory method: Tạo notification mới
        /// </summary>
        public static Notification Create(
            NotificationType type,
            string title,
            string message,
            string? metadataJson = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty", nameof(message));
            return new Notification
            {
                Type = type,
                Title = title,
                Message = message,
                MetadataJson = metadataJson,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Đánh dấu đã đọc
        /// </summary>
        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Đánh dấu chưa đọc
        /// </summary>
        public void MarkAsUnread()
        {
            IsRead = false;
            ReadAt = null;
        }
    }

    /// <summary>
    /// Enum: Loại thông báo
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Thông báo về đơn hàng (tạo mới, xác nhận, giao hàng, hủy)
        /// </summary>
        Order = 1,
        
        /// <summary>
        /// Thông báo về user (đăng ký, active, đổi password)
        /// </summary>
        User = 2,
        
        /// <summary>
        /// Thông báo hệ thống
        /// </summary>
        System = 3
    }
}
