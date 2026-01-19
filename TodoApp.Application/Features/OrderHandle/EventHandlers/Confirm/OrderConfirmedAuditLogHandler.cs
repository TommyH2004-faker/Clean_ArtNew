using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Confirm
{
    /// <summary>
    /// Event Handler: Audit log khi Order được xác nhận
    /// </summary>
    public class OrderConfirmedAuditLogHandler : INotificationHandler<OrderConfirmedEvent>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<OrderConfirmedAuditLogHandler> _logger;

        public OrderConfirmedAuditLogHandler(
            IAuditLogRepository auditLogRepository,
            ILogger<OrderConfirmedAuditLogHandler> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording CONFIRM for Order #{OrderId}", notification.IdOrder);

            var oldValues = JsonSerializer.Serialize(new { Status = "Pending" });
            var newValues = JsonSerializer.Serialize(new
            {
                Status = "Confirmed",
                ConfirmedAt = notification.ConfirmedAt
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
