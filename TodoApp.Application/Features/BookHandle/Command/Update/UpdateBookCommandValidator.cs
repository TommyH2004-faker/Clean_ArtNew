using FluentValidation;
using TodoApp.Application.Features.BookHandle.Command.Update;

namespace TodoApp.Application.Features.BookHandle.Command
{
    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator()
        {
            RuleFor(x => x.IdBook)
                .GreaterThan(0).WithMessage("IdBook must be greater than 0");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required")
                .MaximumLength(200).WithMessage("Author must not exceed 200 characters");

            RuleFor(x => x.NameBook)
                .NotEmpty().WithMessage("Book name is required")
                .MaximumLength(500).WithMessage("Book name must not exceed 500 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.UrlImage)
                .NotEmpty().WithMessage("Image URL is required");

            RuleFor(x => x.ListPrice)
                .GreaterThanOrEqualTo(0).WithMessage("List price cannot be negative");

            RuleFor(x => x.SellPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Sell price cannot be negative")
                .LessThanOrEqualTo(x => x.ListPrice).WithMessage("Sell price cannot exceed list price");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");

            RuleFor(x => x.AvgRating)
                .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5");

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100).WithMessage("Discount percent must be between 0 and 100");
        }
    }
}
