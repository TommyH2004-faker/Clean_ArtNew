using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.OrderDTO;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.OrderHandle.Command.Create
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponseDTO>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IBookRepository bookRepository,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        public async Task<Result<OrderResponseDTO>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Tạo Order
            var order = Orders.Create(request.IdUser, request.Note);

            // 2. Save Order để có IdOrder
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            // 3. Thêm OrderDetails với giá từ Book
            foreach (var item in request.Items)
            {
                // Lấy thông tin Book để có giá
                var book = await _bookRepository.GetBookByIdAsync(item.IdBook);
                if (book == null)
                {
                    _logger.LogWarning("Book #{BookId} not found", item.IdBook);
                    return Result<OrderResponseDTO>.Failure(ErrorType.NotFound, $"Không tìm thấy sách #{item.IdBook}");
                }

                // Kiểm tra số lượng tồn kho
                if (book.Quantity < item.Quantity)
                {
                    _logger.LogWarning("Insufficient quantity for Book #{BookId}", item.IdBook);
                    return Result<OrderResponseDTO>.Failure(ErrorType.Validation, 
                        $"Sách '{book.NameBook}' chỉ còn {book.Quantity} trong kho");
                }

                // Lấy giá bán (SellPrice nếu có, không thì ListPrice)
                var price = book.SellPrice > 0 ? (decimal)book.SellPrice : (decimal)book.ListPrice;

                var orderDetail = OrderDetails.Create(
                    order.IdOrder,
                    item.IdBook,
                    item.Quantity,
                    price
                );
                order.AddOrderDetail(orderDetail);
            }

            // 4. Recalculate total và save
            order.RecalculateTotalPrice();
            
            // 5. Raise Created Event
            order.RaiseCreatedEvent();
            await _orderRepository.SaveChangesAsync(); // Events được dispatch tại đây

            _logger.LogInformation("Order #{OrderId} created successfully", order.IdOrder);

            // 6. Load lại Order với Book details để có BookName
            var orderWithDetails = await _orderRepository.GetByIdWithDetailsAsync(order.IdOrder);

            // 7. Return DTO
            var response = new OrderResponseDTO
            {
                IdOrder = orderWithDetails!.IdOrder,
                IdUser = orderWithDetails.IdUser,
                TotalPrice = orderWithDetails.TotalPrice,
                Status = orderWithDetails.Status,
                Note = orderWithDetails.Note,
                CreatedAt = orderWithDetails.CreatedAt,
                UpdatedAt = orderWithDetails.UpdatedAt,
                OrderDetails = orderWithDetails.OrderDetails.Select(od => new OrderDetailDTO
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
