using MediatR;
using TodoApp.Application.Common;

namespace TodoApp.Application.Features.OrderHandle.Command.Confirm
{
    public record ConfirmOrderCommand : IRequest<Result<string>>
    {
        public int IdOrder { get; init; }
    }
}
