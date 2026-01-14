using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.OrderDTO;

namespace TodoApp.Application.Features.OrderHandle.Query.GetById
{
    public record GetOrderByIdQuery : IRequest<Result<OrderResponseDTO>>
    {
        public int IdOrder { get; init; }
    }
}
