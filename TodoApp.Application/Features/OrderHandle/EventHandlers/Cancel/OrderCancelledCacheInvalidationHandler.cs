using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Cancel
{
    /// <summary>
    /// Event Handler: Xóa cache khi Order bị hủy
    /// </summary>
    public class OrderCancelledCacheInvalidationHandler : INotificationHandler<OrderCancelledEvent>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OrderCancelledCacheInvalidationHandler> _logger;

        public OrderCancelledCacheInvalidationHandler(
            IMemoryCache cache,
            ILogger<OrderCancelledCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(" [CACHE] Xóa cache đơn hàng sau khi hủy - Đơn hàng #{OrderId}", notification.IdOrder);
            
            _cache.Remove("orders:all");
            _cache.Remove($"orders:id:{notification.IdOrder}");
            
            return Task.CompletedTask;
        }
    }
}
