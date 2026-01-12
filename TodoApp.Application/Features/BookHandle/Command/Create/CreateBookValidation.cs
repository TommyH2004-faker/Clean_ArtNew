namespace TodoApp.Application.Features.BookHandle.Command;

using FluentValidation;
using TodoApp.Application.Repository;
public class CreateBookValidation : AbstractValidator<CreateBookCommand>
{
    private readonly BookRepository _bookRepository;
    public CreateBookValidation(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;

        RuleFor(x => x.NameBook)
            .NotEmpty().WithMessage("NameBook is required.")
            .MaximumLength(200).WithMessage("NameBook must not exceed 200 characters.")
            .MustAsync(async (nameBook, cancellation) =>
            {
                var existingBook = await _bookRepository.GetNameBookAsync(nameBook);
                return existingBook == null;
            }).WithMessage("A book with the same NameBook already exists.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MaximumLength(100).WithMessage("Author must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.ListPrice)
            .GreaterThanOrEqualTo(0).WithMessage("ListPrice must be non-negative.");
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative.");
    }
}   