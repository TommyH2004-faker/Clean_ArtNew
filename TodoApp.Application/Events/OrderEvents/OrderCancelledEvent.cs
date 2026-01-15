using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.OrderEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Order Cancelled Domain Event
    /// </summary>
    public class OrderCancelledEvent : IDomainEventWrapper<OrderCancelled>
    {
        public OrderCancelled DomainEvent { get; }
        
        // Explicit interface implementation
        /// <summary>
        /// Lấy Domain Event bên trong wrapper
        /// Lazy load để tránh truy cập không cần thiết
        /// 
        /// 
        /// </summary>
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int IdOrder => DomainEvent.IdOrder;
        public DateTime CancelledAt => DomainEvent.CancelledAt;
        public string Reason => DomainEvent.Reason;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public OrderCancelledEvent(OrderCancelled domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
