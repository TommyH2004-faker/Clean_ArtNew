using MediatR;
using static TodoApp.Domain.Events.GenreEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Genre Created Domain Event
    /// Application layer có thể phụ thuộc MediatR
    /// </summary>
    public class GenreCreatedEvent : INotification
    {
        public GenreCreated DomainEvent { get; }
        
        public int GenreId => DomainEvent.GenreId;
        public string GenreName => DomainEvent.GenreName;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public GenreCreatedEvent(GenreCreated domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}

