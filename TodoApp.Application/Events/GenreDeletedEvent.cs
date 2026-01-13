using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.GenreEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Genre Deleted Domain Event.
    /// Implement IDomainEventWrapper để hỗ trợ auto-discovery.
    /// </summary>
    public class GenreDeletedEvent : IDomainEventWrapper<GenreDeleted>
    {
        public GenreDeleted DomainEvent { get; }
        
        // Explicit interface implementation
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int GenreId => DomainEvent.GenreId;
        public string GenreName => DomainEvent.GenreName;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public GenreDeletedEvent(GenreDeleted domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}

