using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.GenreEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Genre Updated Domain Event.
    /// Implement IDomainEventWrapper để hỗ trợ auto-discovery.
    /// </summary>
    public class GenreUpdatedEvent : IDomainEventWrapper<GenreUpdated>
    {
        public GenreUpdated DomainEvent { get; }
        
        // Explicit interface implementation
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int GenreId => DomainEvent.GenreId;
        public string OldName => DomainEvent.OldName;
        public string NewName => DomainEvent.NewName;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public GenreUpdatedEvent(GenreUpdated domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}

