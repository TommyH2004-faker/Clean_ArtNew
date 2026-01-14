using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers
{
    /// <summary>
    /// Event Handler: Logging cho các Order events
    /// </summary>
    public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;

        public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            var itemCount = notification.OrderDetails.Count;
            var totalItems = notification.OrderDetails.Sum(od => od.Quantity);

            _logger.LogInformation(
                "✅ [ORDER] Order #{OrderId} created by User #{UserId} at {Time} - {ItemCount} items, {TotalItems} total quantity",
                notification.IdOrder,
                notification.IdUser,
                notification.OccurredOn,
                itemCount,
                totalItems);

            // Log chi tiết từng sản phẩm
            foreach (var detail in notification.OrderDetails)
            {
                _logger.LogInformation(
                    "   📦 Book #{BookId}: {Quantity} x {Price:C} = {Subtotal:C}",
                    detail.IdBook,
                    detail.Quantity,
                    detail.Price,
                    detail.Subtotal);
            }

            return Task.CompletedTask;
        }
    }

    public class OrderConfirmedEventHandler : INotificationHandler<OrderConfirmedEvent>
    {
        private readonly ILogger<OrderConfirmedEventHandler> _logger;

        public OrderConfirmedEventHandler(ILogger<OrderConfirmedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "✅ [ORDER] Order #{OrderId} confirmed at {Time}",
                notification.IdOrder,
                notification.ConfirmedAt);

            return Task.CompletedTask;
        }
    }

    public class OrderShippedEventHandler : INotificationHandler<OrderShippedEvent>
    {
        private readonly ILogger<OrderShippedEventHandler> _logger;

        public OrderShippedEventHandler(ILogger<OrderShippedEventHandler> logger)
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

    public class OrderDeliveredEventHandler : INotificationHandler<OrderDeliveredEvent>
    {
        private readonly ILogger<OrderDeliveredEventHandler> _logger;

        public OrderDeliveredEventHandler(ILogger<OrderDeliveredEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderDeliveredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "✅ [ORDER] Order #{OrderId} delivered successfully at {Time}",
                notification.IdOrder,
                notification.DeliveredAt);

            return Task.CompletedTask;
        }
    }

    public class OrderCancelledEventHandler : INotificationHandler<OrderCancelledEvent>
    {
        private readonly ILogger<OrderCancelledEventHandler> _logger;

        public OrderCancelledEventHandler(ILogger<OrderCancelledEventHandler> logger)
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
