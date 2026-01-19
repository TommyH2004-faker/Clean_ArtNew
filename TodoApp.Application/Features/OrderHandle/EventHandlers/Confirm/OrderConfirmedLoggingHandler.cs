using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Confirm
{
    /// <summary>
    /// Event Handler: Logging khi Order được xác nhận
    /// </summary>
    public class OrderConfirmedLoggingHandler : INotificationHandler<OrderConfirmedEvent>
    {
        private readonly ILogger<OrderConfirmedLoggingHandler> _logger;

        public OrderConfirmedLoggingHandler(ILogger<OrderConfirmedLoggingHandler> logger)
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
}
