using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    /// <summary>
    /// Handler chịu trách nhiệm gửi Notifications khi Genre thay đổi.
    /// 
    /// Notifications bao gồm:
    /// - Email cho Admin (qua SendGrid)
    /// - Log cho monitoring
    /// 
    /// Side Effect: Gửi email notification thật!
    /// </summary>
    public class GenreNotificationHandler :
        INotificationHandler<GenreCreatedEvent>,
        INotificationHandler<GenreUpdatedEvent>,
        INotificationHandler<GenreDeletedEvent>
    {
        private readonly ILogger<GenreNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly string[] _adminEmails;

        public GenreNotificationHandler(
            ILogger<GenreNotificationHandler> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            
            // Lấy danh sách email admin từ config
            _adminEmails = configuration.GetSection("AdminEmails").Get<string[]>() 
                ?? new[] { "admin@example.com" };
        }

        /// <summary>
        /// Gửi notification khi Genre mới được tạo
        /// </summary>
        public async Task Handle(GenreCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "📧 [NOTIFICATION] Sending email for new Genre. GenreId: {GenreId}, Name: {GenreName}",
                notification.GenreId,
                notification.GenreName);

            var subject = $"🎉 New Genre Created: {notification.GenreName}";
            var body = BuildGenreCreatedEmailBody(notification);

            foreach (var adminEmail in _adminEmails)
            {
                var success = await _emailService.SendEmailAsync(adminEmail, subject, body, isHtml: true);
                
                if (success)
                {
                    _logger.LogInformation("✅ Email sent to {Email} for Genre creation", adminEmail);
                }
                else
                {
                    _logger.LogWarning("⚠️ Failed to send email to {Email}", adminEmail);
                }
            }
        }

        /// <summary>
        /// Gửi notification khi Genre được cập nhật
        /// </summary>
        public async Task Handle(GenreUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "📧 [NOTIFICATION] Sending email for Genre update. GenreId: {GenreId}, Change: '{OldName}' → '{NewName}'",
                notification.GenreId,
                notification.OldName,
                notification.NewName);

            var subject = $"🔄 Genre Updated: {notification.OldName} → {notification.NewName}";
            var body = BuildGenreUpdatedEmailBody(notification);

            foreach (var adminEmail in _adminEmails)
            {
                await _emailService.SendEmailAsync(adminEmail, subject, body, isHtml: true);
            }
        }

        /// <summary>
        /// Gửi notification khi Genre bị xóa
        /// </summary>
        public async Task Handle(GenreDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "📧 [NOTIFICATION] Sending email for Genre deletion. GenreId: {GenreId}, Name: {GenreName}",
                notification.GenreId,
                notification.GenreName);

            var subject = $"⚠️ Genre DELETED: {notification.GenreName}";
            var body = BuildGenreDeletedEmailBody(notification);

            foreach (var adminEmail in _adminEmails)
            {
                await _emailService.SendEmailAsync(adminEmail, subject, body, isHtml: true);
            }
        }

        #region Email Templates

        private string BuildGenreCreatedEmailBody(GenreCreatedEvent e)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; }}
        .info-box {{ background-color: #f1f1f1; padding: 15px; border-radius: 5px; margin: 10px 0; }}
        .label {{ font-weight: bold; color: #333; }}
        .footer {{ color: #888; font-size: 12px; margin-top: 30px; border-top: 1px solid #ddd; padding-top: 10px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🎉 New Genre Created</h1>
    </div>
    <div class='content'>
        <p>A new genre has been added to the system.</p>
        
        <div class='info-box'>
            <p><span class='label'>Genre ID:</span> {e.GenreId}</p>
            <p><span class='label'>Genre Name:</span> {e.GenreName}</p>
            <p><span class='label'>Created At:</span> {e.OccurredOn:yyyy-MM-dd HH:mm:ss} UTC</p>
        </div>
        
        <p>This is an automated notification from TodoApp.</p>
    </div>
    <div class='footer'>
        <p>TodoApp Notification System | Do not reply to this email</p>
    </div>
</body>
</html>";
        }

        private string BuildGenreUpdatedEmailBody(GenreUpdatedEvent e)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; }}
        .info-box {{ background-color: #f1f1f1; padding: 15px; border-radius: 5px; margin: 10px 0; }}
        .label {{ font-weight: bold; color: #333; }}
        .old-value {{ color: #f44336; text-decoration: line-through; }}
        .new-value {{ color: #4CAF50; font-weight: bold; }}
        .footer {{ color: #888; font-size: 12px; margin-top: 30px; border-top: 1px solid #ddd; padding-top: 10px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🔄 Genre Updated</h1>
    </div>
    <div class='content'>
        <p>A genre has been updated in the system.</p>
        
        <div class='info-box'>
            <p><span class='label'>Genre ID:</span> {e.GenreId}</p>
            <p><span class='label'>Change:</span> 
                <span class='old-value'>{e.OldName}</span> → 
                <span class='new-value'>{e.NewName}</span>
            </p>
            <p><span class='label'>Updated At:</span> {e.OccurredOn:yyyy-MM-dd HH:mm:ss} UTC</p>
        </div>
        
        <p>This is an automated notification from TodoApp.</p>
    </div>
    <div class='footer'>
        <p>TodoApp Notification System | Do not reply to this email</p>
    </div>
</body>
</html>";
        }

        private string BuildGenreDeletedEmailBody(GenreDeletedEvent e)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ background-color: #f44336; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; }}
        .info-box {{ background-color: #ffebee; padding: 15px; border-radius: 5px; margin: 10px 0; border-left: 4px solid #f44336; }}
        .label {{ font-weight: bold; color: #333; }}
        .warning {{ color: #f44336; font-weight: bold; }}
        .footer {{ color: #888; font-size: 12px; margin-top: 30px; border-top: 1px solid #ddd; padding-top: 10px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>⚠️ Genre Deleted</h1>
    </div>
    <div class='content'>
        <p class='warning'>⚠️ A genre has been permanently deleted from the system.</p>
        
        <div class='info-box'>
            <p><span class='label'>Genre ID:</span> {e.GenreId}</p>
            <p><span class='label'>Genre Name:</span> {e.GenreName}</p>
            <p><span class='label'>Deleted At:</span> {e.OccurredOn:yyyy-MM-dd HH:mm:ss} UTC</p>
        </div>
        
        <p>If this was not intentional, please contact the system administrator immediately.</p>
    </div>
    <div class='footer'>
        <p>TodoApp Notification System | Do not reply to this email</p>
    </div>
</body>
</html>";
        }

        #endregion
    }
}
