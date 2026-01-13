using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repository
{
    /// <summary>
    /// Implementation của IAuditLogRepository sử dụng EF Core.
    /// </summary>
    public class AuditLogRepositoryImpl : IAuditLogRepository
    {
        private readonly TodoAppDbContext _context;

        public AuditLogRepositoryImpl(TodoAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
            
            // Lưu ý: Không gọi SaveChanges ở đây!
            // Vì AuditLog được tạo trong EventHandler,
            // và EventHandler chạy SAU khi SaveChanges của entity chính đã hoàn tất.
            // Cần gọi SaveChanges riêng cho AuditLog.
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, string entityId)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync(int page = 1, int pageSize = 50)
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
