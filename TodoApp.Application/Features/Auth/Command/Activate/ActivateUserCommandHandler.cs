using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.Auth.Command.Activate
{
    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ActivateUserCommandHandler> _logger;

        public ActivateUserCommandHandler(IUserRepository userRepository, ILogger<ActivateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            // Lấy user từ database
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("⚠️ [ACTIVATION] User #{UserId} không tồn tại", request.UserId);
                return Result<string>.Failure(ErrorType.NotFound, "Không tìm thấy người dùng");
            }

            // Kiểm tra đã kích hoạt chưa
            if (user.Enabled)
            {
                _logger.LogInformation("ℹ️ [ACTIVATION] User #{UserId} đã được kích hoạt trước đó", request.UserId);
                return Result<string>.Success("Tài khoản đã được kích hoạt trước đó");
            }

            // Kiểm tra mã kích hoạt
            if (user.ActivationCode != request.ActivationCode)
            {
                _logger.LogWarning("❌ [ACTIVATION] Mã kích hoạt không đúng cho user #{UserId}", request.UserId);
                return Result<string>.Failure(ErrorType.Validation, "Mã kích hoạt không đúng");
            }

            // Kích hoạt tài khoản
            user.Activate();
            await _userRepository.SaveChangesAsync(); // Dispatch UserActivated event

            _logger.LogInformation("✅ [ACTIVATION] User #{UserId} ({Email}) đã được kích hoạt thành công", 
                user.IdUser, user.Email);

            return Result<string>.Success("Kích hoạt tài khoản thành công! Bạn có thể đăng nhập ngay.");
        }
    }
}
