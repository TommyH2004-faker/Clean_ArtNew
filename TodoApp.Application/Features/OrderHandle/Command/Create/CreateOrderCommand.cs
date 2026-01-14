using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.OrderDTO;

namespace TodoApp.Application.Features.OrderHandle.Command.Create
{
    public record CreateOrderCommand : IRequest<Result<OrderResponseDTO>>
    {
        public int IdUser { get; init; }
        public string? Note { get; init; }
        public List<CreateOrderItemDTO> Items { get; init; } = new();
    }
}
