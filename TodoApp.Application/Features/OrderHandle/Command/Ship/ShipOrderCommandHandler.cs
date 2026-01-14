using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.OrderHandle.Command.Ship
{
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, Result<string>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ShipOrderCommandHandler> _logger;

        public ShipOrderCommandHandler(
            IOrderRepository orderRepository,
            ILogger<ShipOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.IdOrder);
            if (order == null)
            {
                _logger.LogWarning("Order #{OrderId} not found", request.IdOrder);
                return Result<string>.Failure(ErrorType.NotFound, $"Không tìm thấy đơn hàng #{request.IdOrder}");
            }

            order.StartShipping(request.TrackingNumber);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Order #{OrderId} shipped with tracking: {TrackingNumber}", 
                order.IdOrder, request.TrackingNumber ?? "N/A");
            return Result<string>.Success($"Đơn hàng #{order.IdOrder} đã được giao đi");
        }
    }
}
