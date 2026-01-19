using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Delivery
{
    /// <summary>
    /// Event Handler: Xử lý notification khi Order được giao thành công
    /// </summary>
    public class OrderDeliveredNotificationHandler : INotificationHandler<OrderDeliveredEvent>
    {
        private readonly ILogger<OrderDeliveredNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly string[] _adminEmails;

        public OrderDeliveredNotificationHandler(
            ILogger<OrderDeliveredNotificationHandler> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            _adminEmails = configuration.GetSection("AdminEmails").Get<string[]>() ?? new[] { "admin@example.com" };
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
    }
}
