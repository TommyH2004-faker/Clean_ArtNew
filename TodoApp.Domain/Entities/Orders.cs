using TodoApp.Domain.Common;
using TodoApp.Domain.Events;
using static TodoApp.Domain.Events.OrderEvents;

namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Aggregate Root: Orders (Đơn hàng)
    /// Quản lý toàn bộ lifecycle của một đơn hàng
    /// </summary>
    public class Orders : IHasDomainEvents
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

        // Domain Events Support
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        private Orders() { }

        /// <summary>
        /// Factory method: Tạo đơn hàng mới
        /// KHÔNG raise event ở đây vì IdOrder = 0
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
        /// Raise OrderCreated event SAU khi order đã được save vào DB.
        /// Lúc này IdOrder đã có giá trị thật và OrderDetails đã được thêm.
        /// </summary>
        public void RaiseCreatedEvent()
        {
            var orderDetailsInfo = OrderDetails.Select(od => new OrderDetailInfo(
                od.IdBook,
                od.Quantity,
                od.Price,
                od.Subtotal,
                od.Book?.NameBook ?? string.Empty
            )).ToList();

            AddDomainEvent(new OrderCreated(
                this.IdOrder,
                this.IdUser,
                this.CreatedAt,
                orderDetailsInfo
            ));
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

            // Raise Domain Event
            AddDomainEvent(new OrderConfirmed(this.IdOrder, DateTime.UtcNow));
        }

        /// <summary>
        /// Business logic: Bắt đầu giao hàng
        /// </summary>
        public void StartShipping(string? trackingNumber = null)
        {
            if (Status != "Confirmed")
                throw new InvalidOperationException($"Cannot start shipping for order with status {Status}");

            Status = "Shipping";
            UpdatedAt = DateTime.UtcNow;

            // Raise Domain Event
            AddDomainEvent(new OrderShipped(this.IdOrder, DateTime.UtcNow, trackingNumber));
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

            // Raise Domain Event
            AddDomainEvent(new OrderDelivered(this.IdOrder, DateTime.UtcNow));
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

            // Raise Domain Event
            AddDomainEvent(new OrderCancelled(this.IdOrder, DateTime.UtcNow, reason));
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
