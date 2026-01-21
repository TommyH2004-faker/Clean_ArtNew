
using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.Repository;
namespace TodoApp.Application.Features.OrderHandle.Command.Delivery
{
    public class DeliveryHandle : IRequestHandler<DeliveryCommand, Result<bool>>
    {
        private readonly IOrderRepository _orderRepository;
        public DeliveryHandle(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
      public async Task<Result<bool>> Handle(
            DeliveryCommand request,
            CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetById(request.IdOrder);

            if (order == null)
            {
                return Result<bool>.Failure(ErrorType.NotFound, "Order not found");
            }

            // Đảm bảo trạng thái được cập nhật đúng
            order.CompleteDelivery();

            await _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

    }
}