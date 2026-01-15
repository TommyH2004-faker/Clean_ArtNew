using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.UserEvents;

namespace TodoApp.Application.Events.Auth.Command.UserEvent
{
    public class UserRegisteredEvent : IDomainEventWrapper<UserRegistered>
    {
        public UserRegistered DomainEvent { get; }
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int IdUser => DomainEvent.IdUser;
        public string Email => DomainEvent.Email;
        public string Username => DomainEvent.Username;
        public string ActivationCode => DomainEvent.ActivationCode;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public UserRegisteredEvent(UserRegistered domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}