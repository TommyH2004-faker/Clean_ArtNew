using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.GenreEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Genre Created Domain Event.
    /// Implement IDomainEventWrapper để hỗ trợ auto-discovery.
    /// </summary>
    public class GenreCreatedEvent : IDomainEventWrapper<GenreCreated>
    {
        public GenreCreated DomainEvent { get; }
        
        // Explicit interface implementation
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int GenreId => DomainEvent.GenreId;
        public string GenreName => DomainEvent.GenreName;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public GenreCreatedEvent(GenreCreated domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}

