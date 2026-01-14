using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers
{
    /// <summary>
    /// Event Handler: Audit log cho Order events
    /// </summary>
    public class OrderAuditLogHandler :
        INotificationHandler<OrderCreatedEvent>,
        INotificationHandler<OrderConfirmedEvent>,
        INotificationHandler<OrderShippedEvent>,
        INotificationHandler<OrderDeliveredEvent>,
        INotificationHandler<OrderCancelledEvent>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<OrderAuditLogHandler> _logger;

        public OrderAuditLogHandler(IAuditLogRepository auditLogRepository, ILogger<OrderAuditLogHandler> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording CREATE for Order #{OrderId}", notification.IdOrder);

            var newValues = JsonSerializer.Serialize(new
            {
                notification.IdOrder,
                notification.IdUser,
                notification.OrderDate,
                Status = "Pending",
                OrderDetails = notification.OrderDetails.Select(od => new
                {
                    od.IdBook,
                    od.Quantity,
                    od.Price,
                    od.Subtotal
                })
            });

            var auditLog = AuditLog.Create("CREATE", "Orders", notification.IdOrder.ToString(), null, newValues, $"User#{notification.IdUser}");
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording CONFIRM for Order #{OrderId}", notification.IdOrder);

            var oldValues = JsonSerializer.Serialize(new { Status = "Pending" });
            var newValues = JsonSerializer.Serialize(new { Status = "Confirmed", ConfirmedAt = notification.ConfirmedAt });

            var auditLog = AuditLog.Create("UPDATE", "Orders", notification.IdOrder.ToString(), oldValues, newValues, "System");
            await _auditLogRepository.AddAsync(auditLog);
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

            var auditLog = AuditLog.Create("UPDATE", "Orders", notification.IdOrder.ToString(), oldValues, newValues, "System");
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task Handle(OrderDeliveredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording DELIVERY for Order #{OrderId}", notification.IdOrder);

            var oldValues = JsonSerializer.Serialize(new { Status = "Shipping" });
            var newValues = JsonSerializer.Serialize(new { Status = "Delivered", DeliveredAt = notification.DeliveredAt });

            var auditLog = AuditLog.Create("UPDATE", "Orders", notification.IdOrder.ToString(), oldValues, newValues, "System");
            await _auditLogRepository.AddAsync(auditLog);
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

            var auditLog = AuditLog.Create("UPDATE", "Orders", notification.IdOrder.ToString(), oldValues, newValues, "System");
            await _auditLogRepository.AddAsync(auditLog);
        }
    }
}
