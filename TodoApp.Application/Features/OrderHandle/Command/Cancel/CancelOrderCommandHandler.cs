using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.OrderHandle.Command.Cancel
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<string>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CancelOrderCommandHandler> _logger;

        public CancelOrderCommandHandler(
            IOrderRepository orderRepository,
            ILogger<CancelOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.IdOrder);
            if (order == null)
            {
                _logger.LogWarning("Order #{OrderId} not found", request.IdOrder);
                return Result<string>.Failure(ErrorType.NotFound, $"Không tìm thấy đơn hàng #{request.IdOrder}");
            }

            order.Cancel(request.Reason);
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Order #{OrderId} cancelled: {Reason}", order.IdOrder, request.Reason);
            return Result<string>.Success($"Đơn hàng #{order.IdOrder} đã được hủy");
        }
    }
}
