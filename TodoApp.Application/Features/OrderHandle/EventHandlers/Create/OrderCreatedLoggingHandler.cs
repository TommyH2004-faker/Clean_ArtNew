using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Create
{
    /// <summary>
    /// Event Handler: Logging khi Order được tạo
    /// </summary>
    public class OrderCreatedLoggingHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedLoggingHandler> _logger;

        public OrderCreatedLoggingHandler(ILogger<OrderCreatedLoggingHandler> logger)
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
}
