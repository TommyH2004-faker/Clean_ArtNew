using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Service;

namespace TodoApp.Infrastructure.Services
{
    /// <summary>
    /// Implementation của IEmailService sử dụng SMTP.
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SmtpEmailService(
            IConfiguration configuration,
            ILogger<SmtpEmailService> logger)
        {
            _logger = logger;

            // Lấy config từ appsettings.json
            var smtpHost = configuration["Smtp:Host"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(configuration["Smtp:Port"] ?? "587");
            var smtpUsername = configuration["Smtp:Username"] 
                ?? throw new InvalidOperationException("SMTP Username not configured");
            var smtpPassword = configuration["Smtp:Password"] 
                ?? throw new InvalidOperationException("SMTP Password not configured");
            
            _fromEmail = configuration["Smtp:FromEmail"] ?? smtpUsername;
            _fromName = configuration["Smtp:FromName"] ?? "TodoApp Notification";

            // Cấu hình SMTP client
            _smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,  // Bắt buộc với Gmail/Outlook
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            _logger.LogInformation("📧 SMTP Email Service initialized. Host: {Host}:{Port}, From: {FromEmail}", 
                smtpHost, smtpPort, _fromEmail);
        }

        /// <summary>
        /// Gửi email đơn giản đến 1 người nhận
        /// </summary>
        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                _logger.LogInformation("📧 Sending email to {To} | Subject: {Subject}", to, subject);

                using var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                message.To.Add(new MailAddress(to));

                await _smtpClient.SendMailAsync(message);

                _logger.LogInformation(" Email sent successfully to {To}", to);
                return true;
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, " SMTP Exception while sending email to {To}. StatusCode: {StatusCode}", 
                    to, ex.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Exception while sending email to {To}", to);
                return false;
            }
        }

        /// <summary>
        /// Gửi email đến nhiều người nhận
        /// </summary>
        public async Task<bool> SendEmailAsync(IEnumerable<string> toList, string subject, string body, bool isHtml = true)
        {
            try
            {
                var recipients = toList.ToList();
                _logger.LogInformation("📧 Sending email to {Count} recipients | Subject: {Subject}", 
                    recipients.Count, subject);

                using var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                foreach (var email in recipients)
                {
                    message.To.Add(new MailAddress(email));
                }

                await _smtpClient.SendMailAsync(message);

                _logger.LogInformation("Email sent successfully to {Count} recipients", recipients.Count);
                return true;
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, " SMTP Exception while sending email. StatusCode: {StatusCode}", ex.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Exception while sending email to multiple recipients");
                return false;
            }
        }

        /// <summary>
        /// Gửi email với template (để mở rộng sau)
        /// </summary>
        public async Task<bool> SendEmailWithTemplateAsync(string to, string subject, string templateName, object templateData)
        {
            // Tạo HTML body từ template data
            var body = $@"
                <html>
                <head><style>body {{ font-family: Arial, sans-serif; }}</style></head>
                <body>
                    <h2>{subject}</h2>
                    <p><strong>Template:</strong> {templateName}</p>
                    <pre>{System.Text.Json.JsonSerializer.Serialize(templateData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}</pre>
                </body>
                </html>";
            
            return await SendEmailAsync(to, subject, body, true);
        }

        // Cleanup khi dispose
        public void Dispose()
        {
            _smtpClient?.Dispose();
        }
    }
}
