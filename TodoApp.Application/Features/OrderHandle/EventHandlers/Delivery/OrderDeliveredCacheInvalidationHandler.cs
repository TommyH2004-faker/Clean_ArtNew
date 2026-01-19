using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Delivery
{
    /// <summary>
    /// Event Handler: Xóa cache khi Order được giao thành công
    /// </summary>
    public class OrderDeliveredCacheInvalidationHandler : INotificationHandler<OrderDeliveredEvent>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OrderDeliveredCacheInvalidationHandler> _logger;

        public OrderDeliveredCacheInvalidationHandler(
            IMemoryCache cache,
            ILogger<OrderDeliveredCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task Handle(OrderDeliveredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ [CACHE] Xóa cache đơn hàng sau khi hoàn thành - Đơn hàng #{OrderId}", notification.IdOrder);
            
            _cache.Remove("orders:all");
            _cache.Remove($"orders:id:{notification.IdOrder}");
            
            return Task.CompletedTask;
        }
    }
}
