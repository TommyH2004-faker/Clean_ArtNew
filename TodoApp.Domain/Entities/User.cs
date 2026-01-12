using System.Runtime.InteropServices;

namespace TodoApp.Domain.Entities
{
    public class User
    {
        public int IdUser { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string DeliveryAddress { get; private set; }  = null!;
        public string Email { get; private set; } = null!;
        public string FirstName { get; private set; }  = null!;
        public string LastName { get; private set; } = null!;
        public string Gender { get; private set; } = null!;
        public string Password { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
        public string Username { get; private set; } = null!;
        public string Avatar { get; private set; } = null!;
        public string? ActivationCode { get; private set; } = null;
        public bool? Enabled { get; private set; }
        public string Role { get; set; } = "User";
         public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiry { get; private set; }
        private User() { }
        
        // Method đơn giản cho Register (chỉ cần username, email, password)
        public static User CreateSimple(string username, string email, string passwordHash, string role = "User")
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password cannot be empty", nameof(passwordHash));

            return new User
            {
                Username = username,
                Email = email,
                Password = passwordHash,
                Role = role,
                FirstName = string.Empty,
                LastName = string.Empty,
                PhoneNumber = string.Empty,
                DeliveryAddress = string.Empty,
                Gender = string.Empty,
                Avatar = string.Empty,
                Enabled = true
            };
        }
        
        public static User Create(string firstName, string lastName, string username, string email, 
            string password, string phoneNumber, string deliveryAddress, string gender,string avatar, DateTime? dateOfBirth = null,
            string activationCode = "",  bool? enabled = false)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("FirstName cannot be empty", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("LastName cannot be empty", nameof(lastName));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("PhoneNumber cannot be empty", nameof(phoneNumber));

            if (string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("DeliveryAddress cannot be empty", nameof(deliveryAddress));

            if (string.IsNullOrWhiteSpace(gender))
                throw new ArgumentException ("Gender cannot be empty", nameof(gender));
            if (string.IsNullOrWhiteSpace(avatar))
                throw new ArgumentException("Avatar cannot be empty", nameof(avatar));
            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Email = email,
                Password = password,
                PhoneNumber = phoneNumber,
                DeliveryAddress = deliveryAddress,
                Gender = gender,
                Avatar = avatar,
                DateOfBirth = dateOfBirth,
                ActivationCode = activationCode,
                
            };
        }
        public void UpdateUser(string firstName, string lastName, string username, string email,
            string password, string phoneNumber, string deliveryAddress, string gender, string avatar, DateTime? dateOfBirth = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("FirstName cannot be empty", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("LastName cannot be empty", nameof(lastName));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email format", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("PhoneNumber cannot be empty", nameof(phoneNumber));

            if (string.IsNullOrWhiteSpace(deliveryAddress))
                throw new ArgumentException("DeliveryAddress cannot be empty", nameof(deliveryAddress));

            if (string.IsNullOrWhiteSpace(gender))
                throw new ArgumentException("Gender cannot be empty", nameof(gender));
            if (string.IsNullOrWhiteSpace(avatar))
                throw new ArgumentException("Avatar cannot be empty", nameof(avatar));  
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Username = username;
            this.Email = email;
            this.Password = password;
            this.PhoneNumber = phoneNumber;
            this.DeliveryAddress = deliveryAddress;
            this.Gender = gender;
            this.Avatar = avatar;
            this.DateOfBirth = dateOfBirth;
        }

        public void DeleteUser(int idUser)
        {
            if (this.IdUser != idUser)
            {
                throw new ArgumentException("User ID does not match.", nameof(idUser));
            }
        }
         // Method để update password
        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

            Password = newPasswordHash;
        }

        // Method để update refresh token
        public void SetRefreshToken(string refreshToken, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

            if (expiry <= DateTime.UtcNow)
                throw new ArgumentException("Expiry must be in the future", nameof(expiry));

            RefreshToken = refreshToken;
            RefreshTokenExpiry = expiry;
        }

        // Method để clear refresh token (khi logout)
        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiry = null;
        }
        // Dùng để validate email
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    
    

    }
}