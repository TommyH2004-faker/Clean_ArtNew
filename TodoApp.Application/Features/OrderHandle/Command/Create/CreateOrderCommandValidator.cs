using FluentValidation;

namespace TodoApp.Application.Features.OrderHandle.Command.Create
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.IdUser)
                .GreaterThan(0).WithMessage("User ID must be positive");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must have at least one item")
                .Must(items => items.Count > 0).WithMessage("Order must have at least one item");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.IdBook)
                    .GreaterThan(0).WithMessage("Book ID must be positive");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be positive");
            });
        }
    }
}
