using MediatR;
using static TodoApp.Domain.Events.GenreEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Genre Deleted Domain Event
    /// </summary>
    public class GenreDeletedEvent : INotification
    {
        public GenreDeleted DomainEvent { get; }
        
        public int GenreId => DomainEvent.GenreId;
        public string GenreName => DomainEvent.GenreName;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public GenreDeletedEvent(GenreDeleted domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}

