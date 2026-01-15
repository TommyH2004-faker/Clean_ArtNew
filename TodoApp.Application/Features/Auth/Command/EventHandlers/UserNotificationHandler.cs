using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events.Auth.Command.UserEvent;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.Auth.Command.EventHandlers
{
    public class UserNotificationHandler : 
    INotificationHandler<UserRegisteredEvent>,
    INotificationHandler<UserActivatedEvent>,
    INotificationHandler<UserPasswordChangedEvent>
    {
        private readonly ILogger<UserNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly string _frontendUrl;

        public UserNotificationHandler(
            ILogger<UserNotificationHandler> logger, 
            IEmailService emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            _frontendUrl = configuration["FrontendUrl"] ?? "http://localhost:3000";
        }

        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [USER] Gửi email xác thực cho user {Email}", notification.Email);

            var activationLink = $"{_frontendUrl}/activate?code={notification.ActivationCode}&userId={notification.IdUser}";

            var subject = "🎉 Chào mừng đến BookStore - Kích hoạt tài khoản";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .code-box {{ background: #fff; border: 2px dashed #667eea; padding: 20px; text-align: center; margin: 20px 0; border-radius: 8px; }}
                        .code {{ font-size: 32px; font-weight: bold; color: #667eea; letter-spacing: 5px; }}
                        .button {{ display: inline-block; background: #667eea; color: white !important; padding: 15px 40px; text-decoration: none; border-radius: 25px; margin: 20px 0; font-weight: bold; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <h1>📚 Chào mừng đến BookStore!</h1>
                        </div>
                        <div class=""content"">
                            <h2>Xin chào {notification.Username}! 👋</h2>
                            <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>BookStore</strong>.</p>
                            <p>Để hoàn tất đăng ký, vui lòng kích hoạt tài khoản bằng một trong hai cách:</p>
                            
                            <h3>📋 Cách 1: Nhập mã kích hoạt</h3>
                            <div class=""code-box"">
                                <div class=""code"">{notification.ActivationCode}</div>
                            </div>
                            
                            <h3>🔗 Cách 2: Click vào link bên dưới</h3>
                            <div style=""text-align: center;"">
                                <a href=""{activationLink}"" class=""button"">
                                    ✅ Kích hoạt tài khoản ngay
                                </a>
                            </div>
                            
                            <p style=""margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; font-size: 14px;"">
                                ⚠️ Mã kích hoạt này sẽ hết hạn sau 24 giờ.<br>
                                Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.
                            </p>
                        </div>
                        <div class=""footer"">
                            <p>© 2026 BookStore. All rights reserved.</p>
                            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(notification.Email, subject, body, isHtml: true);
            _logger.LogInformation("✅ [USER] Đã gửi email xác thực thành công cho {Email}", notification.Email);
        }

        public async Task Handle(UserActivatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [USER] Gửi email chúc mừng kích hoạt cho user ID {IdUser}", notification.IdUser);

            var subject = "✅ Tài khoản đã được kích hoạt thành công!";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .success {{ background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%); color: white; padding: 30px; text-align: center; border-radius: 10px; }}
                        .content {{ background: #f9f9f9; padding: 30px; margin-top: 20px; border-radius: 10px; }}
                        .button {{ display: inline-block; background: #11998e; color: white !important; padding: 15px 40px; text-decoration: none; border-radius: 25px; margin: 20px 0; font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""success"">
                            <h1>🎉 Chúc mừng!</h1>
                            <p style=""font-size: 18px; margin: 0;"">Tài khoản của bạn đã được kích hoạt thành công!</p>
                        </div>
                        <div class=""content"">
                            <p>Bây giờ bạn có thể:</p>
                            <ul>
                                <li>📚 Duyệt và mua sách yêu thích</li>
                                <li>⭐ Đánh giá và bình luận</li>
                                <li>❤️ Lưu sách vào danh sách yêu thích</li>
                                <li>🛒 Theo dõi đơn hàng của bạn</li>
                            </ul>
                            <div style=""text-align: center;"">
                                <a href=""{_frontendUrl}/login"" class=""button"">
                                    🚀 Đăng nhập ngay
                                </a>
                            </div>
                        </div>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(notification.Email, subject, body, isHtml: true);
            _logger.LogInformation("✅ [USER] Đã gửi email chúc mừng kích hoạt cho {Email}", notification.Email);
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
            _logger.LogInformation("✅ [USER] Đã gửi email cảnh báo bảo mật cho {Email}", notification.Email);
        }
    }
}