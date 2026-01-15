using TodoApp.Domain.Common;
using TodoApp.Domain.Events;
using static TodoApp.Domain.Events.UserEvents;

namespace TodoApp.Domain.Entities
{
    public class User : IHasDomainEvents
    {
        public int IdUser { get; private set; }

        public string Username { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;

        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string? DeliveryAddress { get; private set; }
        public string? Gender { get; private set; }
        public string? Avatar { get; private set; }
        public DateTime? DateOfBirth { get; private set; }

        public string Role { get; private set; } = "User";
        public string? ActivationCode { get; private set; }
        public bool Enabled { get; private set; } 
        public DateTime? RefreshTokenExpiry { get; private set; }
        public string? RefreshToken { get; private set; }
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

      

        private User() { } // EF

        public static User Register(
        string username,
        string email,
        string passwordHash,
        string role = "User")
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash is required");

            // Tạo activation code 6 số
            var activationCode = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Role = role,
                ActivationCode = activationCode,
                Enabled = false // Chưa kích hoạt
            };

            return user;
        }

        // Raise event SAU khi đã lưu vào DB (có IdUser)
        public void RaiseRegisteredEvent()
        {
            AddDomainEvent(new UserRegistered(
                this.IdUser,
                this.Email,
                this.Username,
                this.ActivationCode!
            ));
        }


        // 👉 Business behavior
        public void UpdateProfile(
            string firstName,
            string lastName,
            string phoneNumber,
            string deliveryAddress,
            string gender,
            string avatar,
            DateTime? dateOfBirth)
        {
            if (!Enabled)
                throw new InvalidOperationException("Account not activated");

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            DeliveryAddress = deliveryAddress;
            Gender = gender;
            Avatar = avatar;
            DateOfBirth = dateOfBirth;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("PasswordHash required");

            PasswordHash = newPasswordHash;
        }

        public void Activate()
        {
            if (Enabled)
                throw new InvalidOperationException("User is already activated");
            
            Enabled = true;
            ActivationCode = null;
            
            AddDomainEvent(new UserActivated(
                this.IdUser,
                this.Email,
                DateTime.UtcNow
            ));
        }

        public void ChangeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role required");

            Role = role;
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void SetRefreshToken(string refreshToken, DateTime refreshTokenExpiry)
        {
            // Lưu refresh token và expiry
            // Giả sử có các thuộc tính RefreshToken và RefreshTokenExpiry
            this.RefreshToken = refreshToken;
            this.RefreshTokenExpiry = refreshTokenExpiry;
        }
    }
}
