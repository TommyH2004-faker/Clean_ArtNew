using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Cancel
{
    /// <summary>
    /// Event Handler: Audit log khi Order bị hủy
    /// </summary>
    public class OrderCancelledAuditLogHandler : INotificationHandler<OrderCancelledEvent>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<OrderCancelledAuditLogHandler> _logger;

        public OrderCancelledAuditLogHandler(
            IAuditLogRepository auditLogRepository,
            ILogger<OrderCancelledAuditLogHandler> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording CANCELLATION for Order #{OrderId}", notification.IdOrder);

            var oldValues = JsonSerializer.Serialize(new { Status = "Previous" });
            var newValues = JsonSerializer.Serialize(new
            {
                Status = "Cancelled",
                CancelledAt = notification.CancelledAt,
                notification.Reason
            });

            var auditLog = AuditLog.Create(
                "UPDATE",
                "Orders",
                notification.IdOrder.ToString(),
                oldValues,
                newValues,
                "System");
            
            await _auditLogRepository.AddAsync(auditLog);
        }
    }
}
