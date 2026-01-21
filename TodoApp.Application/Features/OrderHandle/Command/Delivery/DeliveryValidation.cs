using FluentValidation;

namespace TodoApp.Application.Features.OrderHandle.Command.Delivery
{
    public class DeliveryValidation: AbstractValidator<DeliveryCommand>
    {
        public DeliveryValidation()
        {
            RuleFor(x => x.IdOrder)
                .GreaterThan(0).WithMessage("IdOrder must be greater than 0");

            RuleFor(x => x.DeliveredAt)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("DeliveredAt cannot be in the future");
        }
    }
}