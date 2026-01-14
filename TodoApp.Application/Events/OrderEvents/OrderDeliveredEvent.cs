using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.OrderEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Order Delivered Domain Event
    /// </summary>
    public class OrderDeliveredEvent : IDomainEventWrapper<OrderDelivered>
    {
        public OrderDelivered DomainEvent { get; }
        
        // Explicit interface implementation
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int IdOrder => DomainEvent.IdOrder;
        public DateTime DeliveredAt => DomainEvent.DeliveredAt;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public OrderDeliveredEvent(OrderDelivered domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
