namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity: Feedback (Phản hồi của khách hàng)
    /// Có thể là feedback về hệ thống, dịch vụ, hoặc khiếu nại
    /// </summary>
    public class Feedback
    {
        public int IdFeedback { get; private set; }
        public int IdUser { get; private set; }
        public string Subject { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public string Status { get; private set; } = null!; // Pending, InProgress, Resolved, Closed
        public string? AdminResponse { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? ResolvedAt { get; private set; }

        // Navigation properties
        public User User { get; private set; } = null!;

        private Feedback() { }

        /// <summary>
        /// Factory method: Tạo Feedback mới
        /// </summary>
        public static Feedback Create(int idUser, string subject, string message)
        {
            if (idUser <= 0)
                throw new ArgumentException("User ID must be positive", nameof(idUser));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject cannot be empty", nameof(subject));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty", nameof(message));

            if (subject.Length > 200)
                throw new ArgumentException("Subject cannot exceed 200 characters", nameof(subject));

            return new Feedback
            {
                IdUser = idUser,
                Subject = subject,
                Message = message,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Business logic: Admin bắt đầu xử lý
        /// </summary>
        public void StartProcessing()
        {
            if (Status != "Pending")
                throw new InvalidOperationException($"Cannot start processing feedback with status {Status}");

            Status = "InProgress";
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Admin trả lời feedback
        /// </summary>
        public void Respond(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be empty", nameof(response));

            if (Status == "Closed")
                throw new InvalidOperationException("Cannot respond to closed feedback");

            AdminResponse = response;
            Status = "InProgress";
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Đánh dấu đã giải quyết
        /// </summary>
        public void Resolve()
        {
            if (Status == "Closed")
                throw new InvalidOperationException("Feedback is already closed");

            if (string.IsNullOrWhiteSpace(AdminResponse))
                throw new InvalidOperationException("Cannot resolve feedback without admin response");

            Status = "Resolved";
            ResolvedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Đóng feedback
        /// </summary>
        public void Close()
        {
            if (Status == "Closed")
                throw new InvalidOperationException("Feedback is already closed");

            Status = "Closed";
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Mở lại feedback
        /// </summary>
        public void Reopen(string reason)
        {
            if (Status != "Closed" && Status != "Resolved")
                throw new InvalidOperationException("Can only reopen closed or resolved feedback");

            Status = "Pending";
            AdminResponse = string.IsNullOrEmpty(AdminResponse) 
                ? $"Reopened: {reason}" 
                : $"{AdminResponse}\n\nReopened: {reason}";
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
