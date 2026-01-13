namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity lưu trữ lịch sử thay đổi của các entities trong hệ thống.
    /// Mỗi khi có CREATE/UPDATE/DELETE, một record AuditLog sẽ được tạo.
    /// 
    /// Dùng cho: Audit trail, compliance, debugging, analytics
    /// </summary>
    public class AuditLog
    {
        public int Id { get; private set; }
        
        /// <summary>
        /// Loại hành động: CREATE, UPDATE, DELETE
        /// </summary>
        public string Action { get; private set; } = null!;
        
        /// <summary>
        /// Tên entity bị thay đổi: Genre, Book, User...
        /// </summary>
        public string EntityType { get; private set; } = null!;
        
        /// <summary>
        /// ID của entity bị thay đổi
        /// </summary>
        public string EntityId { get; private set; } = null!;
        
        /// <summary>
        /// Giá trị cũ (JSON) - null nếu là CREATE
        /// </summary>
        public string? OldValues { get; private set; }
        
        /// <summary>
        /// Giá trị mới (JSON) - null nếu là DELETE
        /// </summary>
        public string? NewValues { get; private set; }
        
        /// <summary>
        /// Thời điểm xảy ra hành động
        /// </summary>
        public DateTime Timestamp { get; private set; }
        
        /// <summary>
        /// User thực hiện hành động (nếu có)
        /// </summary>
        public string? PerformedBy { get; private set; }

        // Private constructor cho EF Core
        private AuditLog() { }

        // Factory method
        public static AuditLog Create(
            string action,
            string entityType,
            string entityId,
            string? oldValues,
            string? newValues,
            string? performedBy = null)
        {
            return new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                Timestamp = DateTime.UtcNow,
                PerformedBy = performedBy
            };
        }
    }
}
