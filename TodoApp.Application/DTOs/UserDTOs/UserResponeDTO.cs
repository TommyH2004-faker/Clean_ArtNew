namespace TodoApp.Application.DTOs.UserDTOs
{
    public class  UserResponseDTO
    {
        public int IdUser { get; init; }
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Username { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string PhoneNumber { get; init; } = null!;
        public string DeliveryAddress { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}