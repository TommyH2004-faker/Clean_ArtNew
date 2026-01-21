using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Confirm
{
    /// <summary>
    /// Event Handler: Xử lý notification khi Order được xác nhận
    /// </summary>
    public class OrderConfirmedNotificationHandler : INotificationHandler<OrderConfirmedEvent>
    {
        private readonly ILogger<OrderConfirmedNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly INotification _notification;
        private readonly string[] _adminEmails;

        public OrderConfirmedNotificationHandler(
            ILogger<OrderConfirmedNotificationHandler> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            _adminEmails = configuration.GetSection("AdminEmails").Get<string[]>() ?? new[] { "admin@example.com" };
        }

        public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending email for order confirmation #{OrderId}", notification.IdOrder);

            var subject = $" Order Confirmed: #{notification.IdOrder}";
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
    }
}
