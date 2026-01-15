using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.UserEvents;

namespace TodoApp.Application.Events.Auth.Command.UserEvent
{
    public class UserActivatedEvent : IDomainEventWrapper<UserActivated>
    {
        public UserActivated DomainEvent { get; }
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int IdUser => DomainEvent.IdUser;
        public string Email => DomainEvent.Email;
        public DateTime ActivatedAt => DomainEvent.ActivatedAt;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public UserActivatedEvent(UserActivated domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}