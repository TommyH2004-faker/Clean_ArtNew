using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.OrderDTO;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.OrderHandle.Query.GetById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponseDTO>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(
            IOrderRepository orderRepository,
            ILogger<GetOrderByIdQueryHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<OrderResponseDTO>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(request.IdOrder);
            if (order == null)
            {
                _logger.LogWarning("Order #{OrderId} not found", request.IdOrder);
                return Result<OrderResponseDTO>.Failure(ErrorType.NotFound, $"Không tìm thấy đơn hàng #{request.IdOrder}");
            }

            var response = new OrderResponseDTO
            {
                IdOrder = order.IdOrder,
                IdUser = order.IdUser,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                Note = order.Note,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    IdOrderDetail = od.IdOrderDetail,
                    IdBook = od.IdBook,
                    BookName = od.Book?.NameBook,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Subtotal = od.Subtotal
                }).ToList()
            };

            return Result<OrderResponseDTO>.Success(response);
        }
    }
}
