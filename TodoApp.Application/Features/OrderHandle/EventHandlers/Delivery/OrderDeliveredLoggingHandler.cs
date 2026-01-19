using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Delivery
{
    /// <summary>
    /// Event Handler: Logging khi Order được giao thành công
    /// </summary>
    public class OrderDeliveredLoggingHandler : INotificationHandler<OrderDeliveredEvent>
    {
        private readonly ILogger<OrderDeliveredLoggingHandler> _logger;

        public OrderDeliveredLoggingHandler(ILogger<OrderDeliveredLoggingHandler> logger)
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
}
