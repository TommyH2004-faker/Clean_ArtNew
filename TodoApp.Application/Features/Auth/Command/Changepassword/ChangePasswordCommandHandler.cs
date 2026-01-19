using MediatR;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.Auth.Command.Changepassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
    {
        private readonly IUserRepository _userRepository;

        public ChangePasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // Lấy user từ database
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Kiểm tra user đã kích hoạt chưa
            if (!user.Enabled)
            {
                throw new InvalidOperationException("Account not activated");
            }

            // Xác thực mật khẩu cũ
            var isOldPasswordValid = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
            {
                throw new InvalidOperationException("Old password is incorrect");
            }

            // Hash mật khẩu mới
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Thay đổi mật khẩu (domain method)
            user.ChangePassword(newPasswordHash);

            // ⭐ Raise domain event
            user.RaisePasswordChangedEvent();

            // Lưu thay đổi và dispatch events
            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync(); // Dispatch events

            return new ChangePasswordResponse
            {
                Success = true,
                Message = "Mật khẩu đã được thay đổi thành công!"
            };
        }
    }
}
