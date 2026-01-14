using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.OrderDTO;

namespace TodoApp.Application.Features.OrderHandle.Query.GetAll
{
    public record GetAllOrdersQuery : IRequest<Result<List<OrderResponseDTO>>>
    {
        // Có thể thêm filter parameters sau
        public string? Status { get; init; }
        public int? UserId { get; init; }
    }
}
