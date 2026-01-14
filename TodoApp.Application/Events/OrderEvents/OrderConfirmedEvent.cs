using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.OrderEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Order Confirmed Domain Event
    /// </summary>
    public class OrderConfirmedEvent : IDomainEventWrapper<OrderConfirmed>
    {
        public OrderConfirmed DomainEvent { get; }
        
        // Explicit interface implementation
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int IdOrder => DomainEvent.IdOrder;
        public DateTime ConfirmedAt => DomainEvent.ConfirmedAt;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public OrderConfirmedEvent(OrderConfirmed domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
