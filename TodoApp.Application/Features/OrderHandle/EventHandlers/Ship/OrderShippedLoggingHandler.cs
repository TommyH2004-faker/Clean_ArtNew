using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Ship
{
    /// <summary>
    /// Event Handler: Logging khi Order bắt đầu giao hàng
    /// </summary>
    public class OrderShippedLoggingHandler : INotificationHandler<OrderShippedEvent>
    {
        private readonly ILogger<OrderShippedLoggingHandler> _logger;

        public OrderShippedLoggingHandler(ILogger<OrderShippedLoggingHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderShippedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "🚚 [ORDER] Order #{OrderId} shipped at {Time} - Tracking: {TrackingNumber}",
                notification.IdOrder,
                notification.ShippedAt,
                notification.TrackingNumber ?? "N/A");

            return Task.CompletedTask;
        }
    }
}
