using MediatR;
using TodoApp.Application.Common;

namespace TodoApp.Application.Features.OrderHandle.Command.Cancel
{
    public record CancelOrderCommand : IRequest<Result<string>>
    {
        public int IdOrder { get; init; }
        public string Reason { get; init; } = null!;
    }
}
