using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers
{
    /// <summary>
    /// Event Handler: Email + SignalR notification cho Order events
    /// </summary>
    public class OrderNotificationHandler :
        INotificationHandler<OrderCreatedEvent>,
        INotificationHandler<OrderConfirmedEvent>,
        INotificationHandler<OrderShippedEvent>,
        INotificationHandler<OrderDeliveredEvent>,
        INotificationHandler<OrderCancelledEvent>
    {
        private readonly ILogger<OrderNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly string[] _adminEmails;

        public OrderNotificationHandler(
            ILogger<OrderNotificationHandler> logger,
            IEmailService emailService,
            INotificationService notificationService,
            IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            _notificationService = notificationService;
            _adminEmails = configuration.GetSection("AdminEmails").Get<string[]>() ?? new[] { "admin@example.com" };
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending notifications for new order #{OrderId}", notification.IdOrder);

            var totalAmount = notification.OrderDetails.Sum(od => od.Subtotal);

            // 1. SignalR Realtime Notification (cho chuông 🔔)
            var realtimeNotification = new
            {
                Type = "ORDER_CREATED",
                OrderId = notification.IdOrder,
                UserId = notification.IdUser,
                TotalAmount = totalAmount,
                ItemCount = notification.OrderDetails.Count,
                TotalQuantity = notification.OrderDetails.Sum(od => od.Quantity),
                Timestamp = notification.OrderDate,
                Message = $"Đơn hàng mới #{notification.IdOrder} - {totalAmount:C}",
                Url = $"/admin/orders/{notification.IdOrder}" // URL để redirect khi click notification
            };

            // Gửi đến tất cả admin đang online
            await _notificationService.SendOrderNotificationAsync(realtimeNotification);
            _logger.LogInformation("🔔 [SIGNALR] Sent realtime notification to admins for order #{OrderId}", notification.IdOrder);

            // 2. Email Notification (backup)
            var itemsList = string.Join("<br/>", notification.OrderDetails.Select(od =>
                $"&nbsp;&nbsp;&nbsp;📦 Book #{od.IdBook}: {od.Quantity} x {od.Price:C} = {od.Subtotal:C}"));

            var subject = $"🛒 Đơn hàng mới: #{notification.IdOrder}";
            var body = $@"
                <h2>Đơn hàng mới đã được tạo</h2>
                <p><strong>Mã đơn:</strong> #{notification.IdOrder}</p>
                <p><strong>Khách hàng:</strong> User #{notification.IdUser}</p>
                <p><strong>Thời gian:</strong> {notification.OrderDate:dd/MM/yyyy HH:mm:ss}</p>
                <p><strong>Trạng thái:</strong> Chờ xác nhận</p>
                <hr/>
                <h3>Chi tiết đơn hàng:</h3>
                {itemsList}
                <hr/>
                <p><strong>Tổng tiền:</strong> {totalAmount:C}</p>
                <br/>
                <a href='http://localhost:5000/admin/orders/{notification.IdOrder}' style='padding:10px 20px;background:#007bff;color:white;text-decoration:none;border-radius:5px;'>Xem chi tiết đơn hàng</a>";

            foreach (var email in _adminEmails)
            {
                await _emailService.SendEmailAsync(email, subject, body, isHtml: true);
            }
        }

        public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending email for order confirmation #{OrderId}", notification.IdOrder);

            var subject = $"✅ Order Confirmed: #{notification.IdOrder}";
            var body = $@"
                <h2>Order Confirmed</h2>
                <p><strong>Order ID:</strong> #{notification.IdOrder}</p>
                <p><strong>Confirmed At:</strong> {notification.ConfirmedAt:yyyy-MM-dd HH:mm:ss}</p>
                <p>Your order has been confirmed and is being prepared for shipment.</p>";

            foreach (var email in _adminEmails)
            {
                await _emailService.SendEmailAsync(email, subject, body, isHtml: true);
            }
        }

        public async Task Handle(OrderShippedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending email for order shipment #{OrderId}", notification.IdOrder);

            var subject = $"🚚 Order Shipped: #{notification.IdOrder}";
            var body = $@"
                <h2>Order Shipped</h2>
                <p><strong>Order ID:</strong> #{notification.IdOrder}</p>
                <p><strong>Shipped At:</strong> {notification.ShippedAt:yyyy-MM-dd HH:mm:ss}</p>
                <p><strong>Tracking Number:</strong> {notification.TrackingNumber ?? "N/A"}</p>
                <p>Your order is on the way!</p>";

            foreach (var email in _adminEmails)
            {
                await _emailService.SendEmailAsync(email, subject, body, isHtml: true);
            }
        }

        public async Task Handle(OrderDeliveredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending email for order delivery #{OrderId}", notification.IdOrder);

            var subject = $"✅ Order Delivered: #{notification.IdOrder}";
            var body = $@"
                <h2>Order Delivered Successfully</h2>
                <p><strong>Order ID:</strong> #{notification.IdOrder}</p>
                <p><strong>Delivered At:</strong> {notification.DeliveredAt:yyyy-MM-dd HH:mm:ss}</p>
                <p>Thank you for your purchase! We hope you enjoy your books.</p>
                <p>Please consider leaving a review.</p>";

            foreach (var email in _adminEmails)
            {
                await _emailService.SendEmailAsync(email, subject, body, isHtml: true);
            }
        }

        public async Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending email for order cancellation #{OrderId}", notification.IdOrder);

            var subject = $"❌ Order Cancelled: #{notification.IdOrder}";
            var body = $@"
                <h2>Order Cancelled</h2>
                <p><strong>Order ID:</strong> #{notification.IdOrder}</p>
                <p><strong>Cancelled At:</strong> {notification.CancelledAt:yyyy-MM-dd HH:mm:ss}</p>
                <p><strong>Reason:</strong> {notification.Reason}</p>
                <p>If you have any questions, please contact our support team.</p>";

            foreach (var email in _adminEmails)
            {
                await _emailService.SendEmailAsync(email, subject, body, isHtml: true);
            }
        }
    }
}
