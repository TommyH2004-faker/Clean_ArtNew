using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.BookHandle.Command;

public class CreateBookHandle : IRequestHandler<CreateBookCommand, Result<Book?>>
{
    private readonly BookRepository _bookRepository;
    public CreateBookHandle(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    public async Task<Result<Book>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = Book.Create(
            request.Author,
            request.NameBook,
            request.Description,
            request.ListPrice,
            request.Quantity
        );
        if (book == null)
        {
            return Result<Book>.Failure(ErrorType.Validation, "Dữ liệu sách không hợp lệ");
        }
        await _bookRepository.AddBookAsync(book);
        return Result<Book>.Success(book);
    }
}