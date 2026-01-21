using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events.Auth.Command.UserEvent;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.Auth.EventHandlers.ChangePassword
{
    /// <summary>
    /// Event Handler: Gửi email cảnh báo khi user đổi mật khẩu
    /// </summary>
    public class UserPasswordChangedNotificationHandler : INotificationHandler<UserPasswordChangedEvent>
    {
        private readonly ILogger<UserPasswordChangedNotificationHandler> _logger;
        private readonly IEmailService _emailService;

        public UserPasswordChangedNotificationHandler(
            ILogger<UserPasswordChangedNotificationHandler> logger,
            IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public async Task Handle(UserPasswordChangedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [USER] Gửi email cảnh báo đổi mật khẩu cho user ID {IdUser}", notification.IdUser);

            var subject = "🔒 Mật khẩu đã được thay đổi";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .warning {{ background: #ff6b6b; color: white; padding: 30px; text-align: center; border-radius: 10px; }}
                        .content {{ background: #f9f9f9; padding: 30px; margin-top: 20px; border-radius: 10px; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""warning"">
                            <h1>🔒 Thông báo bảo mật</h1>
                        </div>
                        <div class=""content"">
                            <p>Mật khẩu tài khoản của bạn vừa được thay đổi vào lúc:</p>
                            <p><strong>{notification.ChangedAt:dd/MM/yyyy HH:mm:ss} UTC</strong></p>
                            <p style=""margin-top: 20px; padding: 15px; background: #fff3cd; border-left: 4px solid #ffc107; border-radius: 5px;"">
                                ⚠️ <strong>Nếu không phải bạn thực hiện thay đổi này</strong>, vui lòng liên hệ ngay với chúng tôi để bảo vệ tài khoản.
                            </p>
                        </div>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(notification.Email, subject, body, isHtml: true);
            _logger.LogInformation("[USER] Đã gửi email cảnh báo bảo mật cho {Email}", notification.Email);
        }
    }
}
