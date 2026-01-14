using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.OrderEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Order Shipped Domain Event
    /// </summary>
    public class OrderShippedEvent : IDomainEventWrapper<OrderShipped>
    {
        public OrderShipped DomainEvent { get; }
        
        // Explicit interface implementation
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int IdOrder => DomainEvent.IdOrder;
        public DateTime ShippedAt => DomainEvent.ShippedAt;
        public string? TrackingNumber => DomainEvent.TrackingNumber;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public OrderShippedEvent(OrderShipped domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
