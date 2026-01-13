
using MediatR;
using TodoApp.Application.Repository;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.Auth.Command.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Tìm kiếm user theo email
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Đổi mật khẩu sang hash và so sánh
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Tạo JWT token
            var token = _jwtService.GenerateToken(user);
            var tokenExpiration = _jwtService.GetTokenExpiration();

            // Tạo refresh token
            var refreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiry = _jwtService.GetRefreshTokenExpiration();

            // Lưu refresh token vào database
            user.SetRefreshToken(refreshToken, refreshTokenExpiry);
            await _userRepository.UpdateUserAsync(user);

            return new LoginResponse
            {
                UserId = user.IdUser,
                Username = user.Username,
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = tokenExpiration,
                Role = user.Role
            };
        }
    }
}
