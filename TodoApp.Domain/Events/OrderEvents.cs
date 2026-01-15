using TodoApp.Domain.Common;
using System;
using System.Collections.Generic;

namespace TodoApp.Domain.Events
{
    /// <summary>
    /// Domain Events cho Order aggregate
    /// </summary>
    public static class OrderEvents
    {
        /// <summary>
        /// Event: Đơn hàng mới được tạo
        /// </summary>
        public record OrderCreated : DomainEventBase
        {
            public int IdOrder { get; init; }
            public int IdUser { get; init; }
            public DateTime OrderDate { get; init; }
            public IReadOnlyList<OrderDetailInfo> OrderDetails { get; init; }

            public OrderCreated(int idOrder, int idUser, DateTime orderDate, IReadOnlyList<OrderDetailInfo> orderDetails)
            {
                IdOrder = idOrder;
                IdUser = idUser;
                OrderDate = orderDate;
                OrderDetails = orderDetails;
            }
        }

        /// <summary>
        /// Event: Đơn hàng được xác nhận
        /// </summary>
        public record OrderConfirmed : DomainEventBase
        {
            public int IdOrder { get; init; }
            public DateTime ConfirmedAt { get; init; }
            public OrderConfirmed(int idOrder, DateTime confirmedAt)
            {
                IdOrder = idOrder;
                ConfirmedAt = confirmedAt;
            }
        }

        /// <summary>
        /// Event: Đơn hàng bắt đầu giao hàng
        /// </summary>
        public record OrderShipped : DomainEventBase
        {
            public int IdOrder { get; init; }
            public DateTime ShippedAt { get; init; }
            public string? TrackingNumber { get; init; }
            public OrderShipped(int idOrder, DateTime shippedAt, string? trackingNumber)
            {
                IdOrder = idOrder;
                ShippedAt = shippedAt;
                TrackingNumber = trackingNumber;
            }
        }

        /// <summary>
        /// Event: Đơn hàng đã giao thành công
        /// </summary>
        public record OrderDelivered : DomainEventBase
        {
            public int IdOrder { get; init; }
            public DateTime DeliveredAt { get; init; }
            public OrderDelivered(int idOrder, DateTime deliveredAt)
            {
                IdOrder = idOrder;
                DeliveredAt = deliveredAt;
            }
        }

        /// <summary>
        /// Event: Đơn hàng bị hủy
        /// </summary>
        public record OrderCancelled : DomainEventBase
        {
            public int IdOrder { get; init; }
            public DateTime CancelledAt { get; init; }
            public string Reason { get; init; }
            public OrderCancelled(int idOrder, DateTime cancelledAt, string reason)
            {
                IdOrder = idOrder;
                CancelledAt = cancelledAt;
                Reason = reason;
            }
        }

        /// <summary>
        /// Thông tin chi tiết từng OrderDetail (dùng cho event OrderCreated)
        /// </summary>
        public record OrderDetailInfo
        {
            public int IdBook { get; init; }
            public int Quantity { get; init; }
            public decimal Price { get; init; }
            public decimal Subtotal { get; init; }
            public string NameBook { get; init; }
            public OrderDetailInfo(int idBook, int quantity, decimal price, decimal subtotal, string nameBook)
            {
                IdBook = idBook;
                Quantity = quantity;
                Price = price;
                Subtotal = subtotal;
                NameBook = nameBook;
            }
        }
    }
}