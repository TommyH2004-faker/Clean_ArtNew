using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.OrderEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Order Created Domain Event
    /// </summary>
    public class OrderCreatedEvent : IDomainEventWrapper<OrderCreated>
    {
        public OrderCreated DomainEvent { get; }
        
        // Explicit interface implementation
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int IdOrder => DomainEvent.IdOrder;
        public int IdUser => DomainEvent.IdUser;
        public DateTime OrderDate => DomainEvent.OrderDate;
        public IReadOnlyList<OrderDetailInfo> OrderDetails => DomainEvent.OrderDetails;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public OrderCreatedEvent(OrderCreated domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
