using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Ship
{
    /// <summary>
    /// Event Handler: Xóa cache khi Order bắt đầu giao hàng
    /// </summary>
    public class OrderShippedCacheInvalidationHandler : INotificationHandler<OrderShippedEvent>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OrderShippedCacheInvalidationHandler> _logger;

        public OrderShippedCacheInvalidationHandler(
            IMemoryCache cache,
            ILogger<OrderShippedCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task Handle(OrderShippedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(" [CACHE] Xóa cache đơn hàng sau khi giao hàng - Đơn hàng #{OrderId}", notification.IdOrder);
            
            _cache.Remove("orders:all");
            _cache.Remove($"orders:id:{notification.IdOrder}");
            
            return Task.CompletedTask;
        }
    }
}
