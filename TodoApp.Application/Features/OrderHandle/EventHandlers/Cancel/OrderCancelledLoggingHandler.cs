using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Cancel
{
    /// <summary>
    /// Event Handler: Logging khi Order bị hủy
    /// </summary>
    public class OrderCancelledLoggingHandler : INotificationHandler<OrderCancelledEvent>
    {
        private readonly ILogger<OrderCancelledLoggingHandler> _logger;

        public OrderCancelledLoggingHandler(ILogger<OrderCancelledLoggingHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning(
                "❌ [ORDER] Order #{OrderId} cancelled at {Time} - Reason: {Reason}",
                notification.IdOrder,
                notification.CancelledAt,
                notification.Reason);

            return Task.CompletedTask;
        }
    }
}
