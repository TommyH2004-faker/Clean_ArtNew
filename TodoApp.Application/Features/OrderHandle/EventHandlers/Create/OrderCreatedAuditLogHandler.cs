using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Create
{
    /// <summary>
    /// Event Handler: Audit log khi Order được tạo
    /// </summary>
    public class OrderCreatedAuditLogHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<OrderCreatedAuditLogHandler> _logger;

        public OrderCreatedAuditLogHandler(
            IAuditLogRepository auditLogRepository,
            ILogger<OrderCreatedAuditLogHandler> logger)
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

            var auditLog = AuditLog.Create(
                "CREATE",
                "Orders",
                notification.IdOrder.ToString(),
                null,
                newValues,
                $"User#{notification.IdUser}");
            
            await _auditLogRepository.AddAsync(auditLog);
        }
    }
}
