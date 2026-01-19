using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Ship
{
    /// <summary>
    /// Event Handler: Xử lý notification khi Order bắt đầu giao hàng
    /// </summary>
    public class OrderShippedNotificationHandler : INotificationHandler<OrderShippedEvent>
    {
        private readonly ILogger<OrderShippedNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly string[] _adminEmails;

        public OrderShippedNotificationHandler(
            ILogger<OrderShippedNotificationHandler> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            _adminEmails = configuration.GetSection("AdminEmails").Get<string[]>() ?? new[] { "admin@example.com" };
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
    }
}
