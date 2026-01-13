using TodoApp.Domain.Entities;

namespace TodoApp.Application.Repository
{
    /// <summary>
    /// Repository interface cho AuditLog entity.
    /// Dùng để lưu lịch sử thay đổi của các entities.
    /// </summary>
    public interface IAuditLogRepository
    {
        /// <summary>
        /// Thêm một audit log entry mới
        /// </summary>
        Task AddAsync(AuditLog auditLog);

        /// <summary>
        /// Lấy audit logs theo entity type và ID
        /// </summary>
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, string entityId);

        /// <summary>
        /// Lấy tất cả audit logs (có pagination)
        /// </summary>
        Task<IEnumerable<AuditLog>> GetAllAsync(int page = 1, int pageSize = 50);
    }
}
