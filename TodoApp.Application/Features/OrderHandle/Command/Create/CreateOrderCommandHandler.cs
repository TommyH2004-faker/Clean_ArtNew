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
            // 1. Tạo Order aggregate (chưa có IdOrder)
            var order = Orders.Create(request.IdUser, request.Note);

            // 2. Validate và thêm OrderDetails TRƯỚC KHI save
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

                // ⭐ OrderDetails.IdOrder = 0, EF Core sẽ tự động gán khi SaveChanges
                var orderDetail = OrderDetails.Create(
                    order.IdOrder, // Tạm thời = 0
                    item.IdBook,
                    item.Quantity,
                    price
                );
                order.AddOrderDetail(orderDetail);
            }

            // 3. Recalculate total price
            order.RecalculateTotalPrice();
            
            // 4. Add vào DbContext (tracked but not saved)
            await _orderRepository.AddAsync(order);
            
            // 5. ⭐ SAVE LẦN 1: Order + OrderDetails (ATOMIC trong 1 transaction)
            // EF Core tự động:
            //   - INSERT Order → IdOrder = 123 (từ DB)
            //   - Gán IdOrder = 123 cho tất cả OrderDetails
            //   - INSERT OrderDetails với IdOrder = 123
            await _orderRepository.SaveChangesAsync();
            
            // 6. ⭐ Raise event SAU khi đã có IdOrder
            order.RaiseCreatedEvent();
            
            // 7. ⭐ SAVE LẦN 2: Chỉ dispatch events (không insert thêm data)
            // Entity Order đã tracked, không có thay đổi data
            // → Chỉ trigger event dispatch trong DbContext
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Order #{OrderId} created successfully with {Count} items", 
                order.IdOrder, order.OrderDetails.Count);

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
