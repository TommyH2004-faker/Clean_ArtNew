using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.OrderHandle.Command.Confirm
{
    public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result<string>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ConfirmOrderCommandHandler> _logger;

        public ConfirmOrderCommandHandler(
            IOrderRepository orderRepository,
            ILogger<ConfirmOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.IdOrder);
            if (order == null)
            {
                _logger.LogWarning("Order #{OrderId} not found", request.IdOrder);
                return Result<string>.Failure(ErrorType.NotFound, $"Không tìm thấy đơn hàng #{request.IdOrder}");
            }

            order.Confirm();
            await _orderRepository.SaveChangesAsync();

            _logger.LogInformation("Order #{OrderId} confirmed", order.IdOrder);
            return Result<string>.Success($"Đơn hàng #{order.IdOrder} đã được xác nhận");
        }
    }
}
