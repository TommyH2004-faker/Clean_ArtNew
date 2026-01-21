using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.OrderHandle.EventHandlers.Cancel
{
    /// <summary>
    /// Event Handler: Xử lý notification khi Order bị hủy
    /// </summary>
    public class OrderCancelledNotificationHandler : INotificationHandler<OrderCancelledEvent>
    {
        private readonly ILogger<OrderCancelledNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly string[] _adminEmails;

        public OrderCancelledNotificationHandler(
            ILogger<OrderCancelledNotificationHandler> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            _adminEmails = configuration.GetSection("AdminEmails").Get<string[]>() ?? new[] { "admin@example.com" };
        }

        public async Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending email for order cancellation #{OrderId}", notification.IdOrder);

            var subject = $" Order Cancelled: #{notification.IdOrder}";
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
