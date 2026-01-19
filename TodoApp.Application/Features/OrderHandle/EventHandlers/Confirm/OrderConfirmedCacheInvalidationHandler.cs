using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Confirm
{
    /// <summary>
    /// Event Handler: Xóa cache khi Order được xác nhận
    /// </summary>
    public class OrderConfirmedCacheInvalidationHandler : INotificationHandler<OrderConfirmedEvent>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OrderConfirmedCacheInvalidationHandler> _logger;

        public OrderConfirmedCacheInvalidationHandler(
            IMemoryCache cache,
            ILogger<OrderConfirmedCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ [CACHE] Xóa cache đơn hàng sau khi xác nhận - Đơn hàng #{OrderId}", notification.IdOrder);
            
            _cache.Remove("orders:all");
            _cache.Remove($"orders:id:{notification.IdOrder}");
            
            return Task.CompletedTask;
        }
    }
}
