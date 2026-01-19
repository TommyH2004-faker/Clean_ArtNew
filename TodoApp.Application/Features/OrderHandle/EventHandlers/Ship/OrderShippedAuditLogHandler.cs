using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Ship
{
    /// <summary>
    /// Event Handler: Audit log khi Order bắt đầu giao hàng
    /// </summary>
    public class OrderShippedAuditLogHandler : INotificationHandler<OrderShippedEvent>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<OrderShippedAuditLogHandler> _logger;

        public OrderShippedAuditLogHandler(
            IAuditLogRepository auditLogRepository,
            ILogger<OrderShippedAuditLogHandler> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task Handle(OrderShippedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording SHIP for Order #{OrderId}", notification.IdOrder);

            var oldValues = JsonSerializer.Serialize(new { Status = "Confirmed" });
            var newValues = JsonSerializer.Serialize(new
            {
                Status = "Shipping",
                ShippedAt = notification.ShippedAt,
                notification.TrackingNumber
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
