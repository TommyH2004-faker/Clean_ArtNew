using MediatR;
using TodoApp.Application.Common;

namespace TodoApp.Application.Features.OrderHandle.Command.Delivery
{
    public record DeliveryCommand: IRequest<Result<bool>>
    {
        public int IdOrder { get; init; }
        public DateTime DeliveredAt { get; init; }
    }
}