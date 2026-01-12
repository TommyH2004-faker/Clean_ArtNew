using MediatR;
using TodoApp.Application.Features.Auth.Command.Login;

namespace TodoApp.Application.Features.Auth.Command.Refresh 
{
    public class RefreshTokenCommand : IRequest<LoginResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}