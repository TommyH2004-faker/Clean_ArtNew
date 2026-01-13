namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity: Delivery (Giao hàng)
    /// Entity thuộc Orders aggregate
    /// </summary>
    public class Delivery
    {
        public int IdDelivery { get; private set; }
        public int IdOrder { get; private set; }
        public string DeliveryAddress { get; private set; } = null!;
        public string ReceiverName { get; private set; } = null!;
        public string ReceiverPhone { get; private set; } = null!;
        public string Status { get; private set; } = null!; // Preparing, InTransit, Delivered, Failed
        public DateTime? ShippedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public string? TrackingNumber { get; private set; }
        public string? Note { get; private set; }

        // Navigation properties
        public Orders Order { get; private set; } = null!;

        private Delivery() { }

        /// <summary>
        /// Factory method: Tạo Delivery mới
        /// </summary>
        public static Delivery Create(int idOrder, string deliveryAddress, string receiverName, string receiverPhone, string? note = null)
        {
            if (idOrder <= 0)
                throw new ArgumentException("Order ID must be positive", nameof(idOrder));

            if (string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("Delivery address cannot be empty", nameof(deliveryAddress));

            if (string.IsNullOrWhiteSpace(receiverName))
                throw new ArgumentException("Receiver name cannot be empty", nameof(receiverName));

            if (string.IsNullOrWhiteSpace(receiverPhone))
                throw new ArgumentException("Receiver phone cannot be empty", nameof(receiverPhone));

            return new Delivery
            {
                IdOrder = idOrder,
                DeliveryAddress = deliveryAddress,
                ReceiverName = receiverName,
                ReceiverPhone = receiverPhone,
                Status = "Preparing",
                Note = note
            };
        }

        /// <summary>
        /// Business logic: Bắt đầu vận chuyển
        /// </summary>
        public void StartShipping(string? trackingNumber = null)
        {
            if (Status != "Preparing")
                throw new InvalidOperationException($"Cannot start shipping from status {Status}");

            Status = "InTransit";
            TrackingNumber = trackingNumber;
            ShippedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Hoàn thành giao hàng
        /// </summary>
        public void CompleteDelivery()
        {
            if (Status != "InTransit")
                throw new InvalidOperationException($"Cannot complete delivery from status {Status}");

            Status = "Delivered";
            DeliveredAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Đánh dấu thất bại
        /// </summary>
        public void MarkAsFailed(string reason)
        {
            if (Status == "Delivered")
                throw new InvalidOperationException("Cannot fail a delivered order");

            Status = "Failed";
            Note = string.IsNullOrEmpty(Note) ? reason : $"{Note}\nFailure: {reason}";
        }

        /// <summary>
        /// Business logic: Cập nhật địa chỉ
        /// </summary>
        public void UpdateAddress(string newAddress)
        {
            if (Status != "Preparing")
                throw new InvalidOperationException("Cannot update address after shipping started");

            if (string.IsNullOrWhiteSpace(newAddress))
                throw new ArgumentException("Address cannot be empty", nameof(newAddress));

            DeliveryAddress = newAddress;
        }

        /// <summary>
        /// Business logic: Cập nhật tracking number
        /// </summary>
        public void UpdateTrackingNumber(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty", nameof(trackingNumber));

            TrackingNumber = trackingNumber;
        }
    }
}
