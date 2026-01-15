using TodoApp.Domain.Common;

namespace TodoApp.Domain.Events
{
    public static class UserEvents
    {
        public record UserRegistered : DomainEventBase
        {
            public int IdUser { get; init; }
            public string Email { get; init; }
            public string Username { get; init; }
            public string ActivationCode { get; init; }
            
            public UserRegistered(int idUser, string email, string username, string activationCode)
            {
                IdUser = idUser;
                Email = email;
                Username = username;
                ActivationCode = activationCode;
            }
        }

        public record UserActivated : DomainEventBase
        {
            public int IdUser { get; init; }
            public string Email { get; init; }
            public DateTime ActivatedAt { get; init; }
            
            public UserActivated(int idUser, string email, DateTime activatedAt)
            {
                IdUser = idUser;
                Email = email;
                ActivatedAt = activatedAt;
            }
        }

        public record UserProfileUpdated : DomainEventBase
        {
            public int IdUser { get; init; }
            
            public UserProfileUpdated(int idUser)
            {
                IdUser = idUser;
            }
        }

        public record UserPasswordChanged : DomainEventBase
        {
            public int IdUser { get; init; }
            public string Email { get; init; }
            public DateTime ChangedAt { get; init; }
            
            public UserPasswordChanged(int idUser, string email, DateTime changedAt)
            {
                IdUser = idUser;
                Email = email;
                ChangedAt = changedAt;
            }
        }
    }
}