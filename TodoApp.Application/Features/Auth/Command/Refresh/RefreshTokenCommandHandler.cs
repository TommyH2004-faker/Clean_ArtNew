using MediatR;
using TodoApp.Application.Features.Auth.Command.Login;
using TodoApp.Application.Repository;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.Auth.Command.Refresh;
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public RefreshTokenCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Tìm user có refresh token này
            var users = await _userRepository.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.RefreshToken == request.RefreshToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Kiểm tra refresh token còn hạn không
            if (user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token expired");
            }

            // Tạo JWT token mới
            var newToken = _jwtService.GenerateToken(user);
            var tokenExpiration = _jwtService.GetTokenExpiration();

            // Tạo refresh token mới (rotation strategy - bảo mật cao hơn)
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var refreshTokenExpiry = _jwtService.GetRefreshTokenExpiration();

            // Cập nhật refresh token mới vào DB
            user.SetRefreshToken(newRefreshToken, refreshTokenExpiry);
            await _userRepository.UpdateUserAsync(user);

            return new LoginResponse
            {
                UserId = user.IdUser,
                Username = user.Username,
                Email = user.Email,
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = tokenExpiration,
                Role = user.Role
            };
        }
}