using MediatR;
using TodoApp.Application.Common;

namespace TodoApp.Application.Features.Auth.Command.Activate
{
    public record ActivateUserCommand : IRequest<Result<string>>
    {
        public int UserId { get; init; }
        public string ActivationCode { get; init; } = string.Empty;
    }
}
