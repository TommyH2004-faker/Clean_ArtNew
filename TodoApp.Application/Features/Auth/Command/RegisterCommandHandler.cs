using MediatR;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.Auth.Command
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly UserRepository _userRepository;

        public RegisterCommandHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra email đã tồn tại chưa
            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Kiểm tra username đã tồn tại chưa
            var existingUserByUsername = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUserByUsername != null)
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Hash password với BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Tạo user mới sử dụng factory method
            var newUser = User.CreateSimple(
                username: request.Username,
                email: request.Email,
                passwordHash: hashedPassword,
                role: "User"
            );

            // Lưu vào database
            await _userRepository.AddUserAsync(newUser);

            return new RegisterResponse
            {
                UserId = newUser.IdUser,
                Username = newUser.Username,
                Email = newUser.Email,
                Message = "User registered successfully"
            };
        }
    }
}
