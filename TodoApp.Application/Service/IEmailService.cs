namespace TodoApp.Application.Service
{
    /// <summary>
    /// Interface cho Email Service.
    /// Dùng để gửi email notifications trong các Event Handlers.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email đơn giản
        /// </summary>
        /// <param name="to">Email người nhận</param>
        /// <param name="subject">Tiêu đề</param>
        /// <param name="body">Nội dung (HTML hoặc plain text)</param>
        /// <param name="isHtml">Có phải HTML không</param>
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Gửi email đến nhiều người
        /// </summary>
        Task<bool> SendEmailAsync(IEnumerable<string> toList, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Gửi email với template
        /// </summary>
        Task<bool> SendEmailWithTemplateAsync(string to, string subject, string templateName, object templateData);
    }

    /// <summary>
    /// DTO cho email message
    /// </summary>
    public class EmailMessage
    {
        public string To { get; set; } = null!;
        public List<string> ToList { get; set; } = new();
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public bool IsHtml { get; set; } = true;
    }
}
