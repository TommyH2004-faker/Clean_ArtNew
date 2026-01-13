using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    /// <summary>
    /// Handler chịu trách nhiệm ghi Audit Log khi Genre thay đổi.
    /// 
    /// Audit Log giúp:
    /// - Theo dõi ai đã làm gì, khi nào
    /// - Compliance với regulations (GDPR, SOX, etc.)
    /// - Debugging và troubleshooting
    /// - Analytics về thay đổi dữ liệu
    /// 
    /// Side Effect: Ghi vào bảng AuditLogs
    /// </summary>
    public class GenreAuditLogHandler :
        INotificationHandler<GenreCreatedEvent>,
        INotificationHandler<GenreUpdatedEvent>,
        INotificationHandler<GenreDeletedEvent>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<GenreAuditLogHandler> _logger;

        public GenreAuditLogHandler(
            IAuditLogRepository auditLogRepository,
            ILogger<GenreAuditLogHandler> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        /// <summary>
        /// Ghi audit log khi Genre được tạo mới
        /// </summary>
        public async Task Handle(GenreCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "📝 [AUDIT] Recording CREATE action for Genre. GenreId: {GenreId}, Name: {GenreName}",
                notification.GenreId,
                notification.GenreName);

            var newValues = JsonSerializer.Serialize(new
            {
                notification.GenreId,
                notification.GenreName,
                notification.OccurredOn
            });

            var auditLog = AuditLog.Create(
                action: "CREATE",
                entityType: "Genre",
                entityId: notification.GenreId.ToString(),
                oldValues: null,  // Không có giá trị cũ khi CREATE
                newValues: newValues,
                performedBy: "System" // TODO: Lấy từ HttpContext.User nếu cần
            );

            await _auditLogRepository.AddAsync(auditLog);

            _logger.LogDebug(
                "📝 [AUDIT] Audit log saved. Action: CREATE, EntityType: Genre, EntityId: {GenreId}",
                notification.GenreId);
        }

        /// <summary>
        /// Ghi audit log khi Genre được cập nhật
        /// </summary>
        public async Task Handle(GenreUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "📝 [AUDIT] Recording UPDATE action for Genre. GenreId: {GenreId}, OldName: {OldName} → NewName: {NewName}",
                notification.GenreId,
                notification.OldName,
                notification.NewName);

            var oldValues = JsonSerializer.Serialize(new
            {
                notification.GenreId,
                GenreName = notification.OldName
            });

            var newValues = JsonSerializer.Serialize(new
            {
                notification.GenreId,
                GenreName = notification.NewName,
                notification.OccurredOn
            });

            var auditLog = AuditLog.Create(
                action: "UPDATE",
                entityType: "Genre",
                entityId: notification.GenreId.ToString(),
                oldValues: oldValues,
                newValues: newValues,
                performedBy: "System"
            );

            await _auditLogRepository.AddAsync(auditLog);

            _logger.LogDebug(
                "📝 [AUDIT] Audit log saved. Action: UPDATE, EntityType: Genre, EntityId: {GenreId}",
                notification.GenreId);
        }

        /// <summary>
        /// Ghi audit log khi Genre bị xóa
        /// </summary>
        public async Task Handle(GenreDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "📝 [AUDIT] Recording DELETE action for Genre. GenreId: {GenreId}, Name: {GenreName}",
                notification.GenreId,
                notification.GenreName);

            var oldValues = JsonSerializer.Serialize(new
            {
                notification.GenreId,
                notification.GenreName
            });

            var auditLog = AuditLog.Create(
                action: "DELETE",
                entityType: "Genre",
                entityId: notification.GenreId.ToString(),
                oldValues: oldValues,
                newValues: null,  // Không có giá trị mới khi DELETE
                performedBy: "System"
            );

            await _auditLogRepository.AddAsync(auditLog);

            _logger.LogDebug(
                "📝 [AUDIT] Audit log saved. Action: DELETE, EntityType: Genre, EntityId: {GenreId}",
                notification.GenreId);
        }
    }
}
