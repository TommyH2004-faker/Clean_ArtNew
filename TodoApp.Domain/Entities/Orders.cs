namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Aggregate Root: Orders (Đơn hàng)
    /// Quản lý toàn bộ lifecycle của một đơn hàng
    /// </summary>
    public class Orders
    {
        public int IdOrder { get; private set; }
        public int IdUser { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string Status { get; private set; } = null!; // Pending, Confirmed, Shipping, Delivered, Cancelled
        public string? Note { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation properties
        public User User { get; private set; } = null!;
        public ICollection<OrderDetails> OrderDetails { get; private set; } = new List<OrderDetails>();
        public Payment? Payment { get; private set; }
        public Delivery? Delivery { get; private set; }

        private Orders() { }

        /// <summary>
        /// Factory method: Tạo đơn hàng mới
        /// </summary>
        public static Orders Create(int idUser, string? note = null)
        {
            if (idUser <= 0)
                throw new ArgumentException("User ID must be positive", nameof(idUser));

            return new Orders
            {
                IdUser = idUser,
                TotalPrice = 0,
                Status = "Pending",
                Note = note,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Business logic: Thêm OrderDetail
        /// </summary>
        public void AddOrderDetail(OrderDetails orderDetail)
        {
            if (Status != "Pending")
                throw new InvalidOperationException("Cannot modify order that is not pending");

            OrderDetails.Add(orderDetail);
            RecalculateTotalPrice();
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Tính lại tổng giá
        /// </summary>
        public void RecalculateTotalPrice()
        {
            TotalPrice = OrderDetails.Sum(od => od.Quantity * od.Price);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Xác nhận đơn hàng
        /// </summary>
        public void Confirm()
        {
            if (Status != "Pending")
                throw new InvalidOperationException($"Cannot confirm order with status {Status}");

            if (!OrderDetails.Any())
                throw new InvalidOperationException("Cannot confirm order without items");

            Status = "Confirmed";
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Bắt đầu giao hàng
        /// </summary>
        public void StartShipping()
        {
            if (Status != "Confirmed")
                throw new InvalidOperationException($"Cannot start shipping for order with status {Status}");

            Status = "Shipping";
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Hoàn thành giao hàng
        /// </summary>
        public void CompleteDelivery()
        {
            if (Status != "Shipping")
                throw new InvalidOperationException($"Cannot complete delivery for order with status {Status}");

            Status = "Delivered";
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Hủy đơn hàng
        /// </summary>
        public void Cancel(string reason)
        {
            if (Status == "Delivered")
                throw new InvalidOperationException("Cannot cancel delivered order");

            if (Status == "Cancelled")
                throw new InvalidOperationException("Order is already cancelled");

            Status = "Cancelled";
            Note = string.IsNullOrEmpty(Note) ? reason : $"{Note}\nCancellation: {reason}";
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Cập nhật note
        /// </summary>
        public void UpdateNote(string note)
        {
            Note = note;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
