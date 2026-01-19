using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Delivery
{
    /// <summary>
    /// Event Handler: Audit log khi Order được giao thành công
    /// </summary>
    public class OrderDeliveredAuditLogHandler : INotificationHandler<OrderDeliveredEvent>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<OrderDeliveredAuditLogHandler> _logger;

        public OrderDeliveredAuditLogHandler(
            IAuditLogRepository auditLogRepository,
            ILogger<OrderDeliveredAuditLogHandler> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task Handle(OrderDeliveredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording DELIVERY for Order #{OrderId}", notification.IdOrder);

            var oldValues = JsonSerializer.Serialize(new { Status = "Shipping" });
            var newValues = JsonSerializer.Serialize(new
            {
                Status = "Delivered",
                DeliveredAt = notification.DeliveredAt
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
