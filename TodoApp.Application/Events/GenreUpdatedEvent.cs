using MediatR;
using static TodoApp.Domain.Events.GenreEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Genre Updated Domain Event
    /// </summary>
    public class GenreUpdatedEvent : INotification
    {
        public GenreUpdated DomainEvent { get; }
        
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

