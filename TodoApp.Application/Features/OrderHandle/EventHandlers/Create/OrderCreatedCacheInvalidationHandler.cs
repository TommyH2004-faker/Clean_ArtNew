using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Create
{
    /// <summary>
    /// Event Handler: Xóa cache khi Order được tạo
    /// </summary>
    public class OrderCreatedCacheInvalidationHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OrderCreatedCacheInvalidationHandler> _logger;

        public OrderCreatedCacheInvalidationHandler(
            IMemoryCache cache,
            ILogger<OrderCreatedCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ [CACHE] Xóa cache đơn hàng sau khi tạo - Đơn hàng #{OrderId}", notification.IdOrder);
            
            _cache.Remove("orders:all");
            _cache.Remove($"orders:id:{notification.IdOrder}");
            _cache.Remove($"orders:user:{notification.IdUser}");
            
            return Task.CompletedTask;
        }
    }
}
