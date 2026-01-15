using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers
{
    /// <summary>
    /// Event Handler: Xóa cache cho các sự kiện đơn hàng
    /// </summary>
    public class OrderCacheInvalidationHandler :
        INotificationHandler<OrderCreatedEvent>,
        INotificationHandler<OrderConfirmedEvent>,
        INotificationHandler<OrderShippedEvent>,
        INotificationHandler<OrderDeliveredEvent>,
        INotificationHandler<OrderCancelledEvent>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<OrderCacheInvalidationHandler> _logger;
        private const string ALL_ORDERS_CACHE_KEY = "orders:all";
        private const string USER_ORDERS_CACHE_PREFIX = "orders:user:";

        public OrderCacheInvalidationHandler(IMemoryCache cache, ILogger<OrderCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ [CACHE] Xóa cache đơn hàng sau khi tạo - Đơn hàng #{OrderId}", notification.IdOrder);
            ClearCache(notification.IdOrder, notification.IdUser);
            return Task.CompletedTask;
        }

        public Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ [CACHE] Xóa cache đơn hàng sau khi xác nhận - Đơn hàng #{OrderId}", notification.IdOrder);
            ClearCache(notification.IdOrder, null);
            return Task.CompletedTask;
        }

        public Task Handle(OrderShippedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ [CACHE] Xóa cache đơn hàng sau khi giao hàng - Đơn hàng #{OrderId}", notification.IdOrder);
            ClearCache(notification.IdOrder, null);
            return Task.CompletedTask;
        }

        public Task Handle(OrderDeliveredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ [CACHE] Xóa cache đơn hàng sau khi hoàn thành - Đơn hàng #{OrderId}", notification.IdOrder);
            ClearCache(notification.IdOrder, null);
            return Task.CompletedTask;
        }

        public Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🗑️ [CACHE] Xóa cache đơn hàng sau khi hủy - Đơn hàng #{OrderId}", notification.IdOrder);
            ClearCache(notification.IdOrder, null);
            return Task.CompletedTask;
        }

        private void ClearCache(int orderId, int? userId)
        {
            _cache.Remove(ALL_ORDERS_CACHE_KEY);
            _cache.Remove($"orders:id:{orderId}");
            
            if (userId.HasValue)
            {
                _cache.Remove($"{USER_ORDERS_CACHE_PREFIX}{userId}");
            }
        }
    }
}
