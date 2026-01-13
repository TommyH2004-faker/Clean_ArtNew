namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity: Payment (Thanh toán)
    /// Entity thuộc Orders aggregate hoặc có thể là aggregate riêng
    /// </summary>
    public class Payment
    {
        public int IdPayment { get; private set; }
        public int IdOrder { get; private set; }
        public string PaymentMethod { get; private set; } = null!; // COD, CreditCard, BankTransfer, Momo, ZaloPay
        public decimal Amount { get; private set; }
        public string Status { get; private set; } = null!; // Pending, Completed, Failed, Refunded
        public string? TransactionId { get; private set; } // ID từ payment gateway
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public string? Note { get; private set; }

        // Navigation properties
        public Orders Order { get; private set; } = null!;

        private Payment() { }

        /// <summary>
        /// Factory method: Tạo Payment mới
        /// </summary>
        public static Payment Create(int idOrder, string paymentMethod, decimal amount, string? note = null)
        {
            if (idOrder <= 0)
                throw new ArgumentException("Order ID must be positive", nameof(idOrder));

            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new ArgumentException("Payment method cannot be empty", nameof(paymentMethod));

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");

            // Validate payment method
            var validMethods = new[] { "COD", "CreditCard", "BankTransfer", "Momo", "ZaloPay" };
            if (!validMethods.Contains(paymentMethod))
                throw new ArgumentException($"Invalid payment method. Valid methods: {string.Join(", ", validMethods)}", nameof(paymentMethod));

            return new Payment
            {
                IdOrder = idOrder,
                PaymentMethod = paymentMethod,
                Amount = amount,
                Status = "Pending",
                Note = note,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Business logic: Hoàn thành thanh toán
        /// </summary>
        public void Complete(string? transactionId = null)
        {
            if (Status == "Completed")
                throw new InvalidOperationException("Payment is already completed");

            if (Status == "Refunded")
                throw new InvalidOperationException("Cannot complete a refunded payment");

            Status = "Completed";
            TransactionId = transactionId;
            CompletedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Đánh dấu thất bại
        /// </summary>
        public void MarkAsFailed(string reason)
        {
            if (Status == "Completed")
                throw new InvalidOperationException("Cannot fail a completed payment");

            Status = "Failed";
            Note = string.IsNullOrEmpty(Note) ? reason : $"{Note}\nFailure: {reason}";
        }

        /// <summary>
        /// Business logic: Hoàn tiền
        /// </summary>
        public void Refund(string reason)
        {
            if (Status != "Completed")
                throw new InvalidOperationException("Can only refund completed payments");

            Status = "Refunded";
            Note = string.IsNullOrEmpty(Note) ? reason : $"{Note}\nRefund: {reason}";
        }

        /// <summary>
        /// Business logic: Cập nhật Transaction ID
        /// </summary>
        public void UpdateTransactionId(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID cannot be empty", nameof(transactionId));

            TransactionId = transactionId;
        }
    }
}
