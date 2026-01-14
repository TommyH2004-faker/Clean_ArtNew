using MediatR;
using TodoApp.Application.Common;

namespace TodoApp.Application.Features.OrderHandle.Command.Ship
{
    public record ShipOrderCommand : IRequest<Result<string>>
    {
        public int IdOrder { get; init; }
        public string? TrackingNumber { get; init; }
    }
}
