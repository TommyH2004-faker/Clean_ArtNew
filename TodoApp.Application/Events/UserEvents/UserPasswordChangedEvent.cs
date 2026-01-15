using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.UserEvents;

namespace TodoApp.Application.Events.Auth.Command.UserEvent
{
    public class UserPasswordChangedEvent : IDomainEventWrapper<UserPasswordChanged>
    {
        public UserPasswordChanged DomainEvent { get; }
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int IdUser => DomainEvent.IdUser;
        public string Email => DomainEvent.Email;
        public DateTime ChangedAt => DomainEvent.ChangedAt;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public UserPasswordChangedEvent(UserPasswordChanged domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}