using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.OrderDTO;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.OrderHandle.Query.GetAll
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<List<OrderResponseDTO>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetAllOrdersQueryHandler> _logger;

        public GetAllOrdersQueryHandler(
            IOrderRepository orderRepository,
            ILogger<GetAllOrdersQueryHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<List<OrderResponseDTO>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var orders = await _orderRepository.GetAllWithDetailsAsync();

                // Apply filters if provided
                if (!string.IsNullOrEmpty(request.Status))
                {
                    orders = orders.Where(o => o.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (request.UserId.HasValue)
                {
                    orders = orders.Where(o => o.IdUser == request.UserId.Value).ToList();
                }

                var response = orders.Select(order => new OrderResponseDTO
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
                }).OrderByDescending(o => o.CreatedAt).ToList();

                _logger.LogInformation("Retrieved {Count} orders", response.Count);
                return Result<List<OrderResponseDTO>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return Result<List<OrderResponseDTO>>.Failure(ErrorType.Failure, "Lỗi khi lấy danh sách đơn hàng");
            }
        }
    }
}
