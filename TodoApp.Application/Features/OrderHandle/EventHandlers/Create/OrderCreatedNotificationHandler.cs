using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Create
{
    /// <summary>
    /// Event Handler: Xử lý notification khi Order được tạo mới
    /// </summary>
    public class OrderCreatedNotificationHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly string[] _adminEmails;

        public OrderCreatedNotificationHandler(
            ILogger<OrderCreatedNotificationHandler> logger,
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
            var title = $"🛒 Đơn hàng mới #{notification.IdOrder}";
            var message = $"Khách hàng đã đặt đơn hàng trị giá {totalAmount:C} với {notification.OrderDetails.Count} sản phẩm";
            var metadata = new
            {
                type = "ORDER_CREATED",
                orderId = notification.IdOrder,
                userId = notification.IdUser,
                totalAmount = totalAmount,
                itemCount = notification.OrderDetails.Count,
                totalQuantity = notification.OrderDetails.Sum(od => od.Quantity),
                timestamp = notification.OrderDate,
                url = $"/admin/orders/{notification.IdOrder}",
                details = notification.OrderDetails.Select(od => new
                {
                    bookId = od.IdBook,
                    bookName = od.NameBook,
                    quantity = od.Quantity,
                    price = od.Price,
                    subtotal = od.Subtotal
                })
            };

            // Lưu vào DB + Push đến tất cả admin đang online
            await _notificationService.SendOrderNotificationAsync(title, message, metadata);
            _logger.LogInformation("🔔 [SIGNALR] Sent realtime notification to admins for order #{OrderId}", notification.IdOrder);

            // 2. Email Notification (backup)
            var itemsList = string.Join("<br/>", notification.OrderDetails.Select(od =>
                $"&nbsp;&nbsp;&nbsp;📦 Book #{od.IdBook}: {od.Quantity} x {od.Price:C} = {od.Subtotal:C} x {od.NameBook}"));

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
    }
}
