using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.BookHandle.Queries.GetIdBook;

public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, Result<Book?>>
{
    private readonly BookRepository _bookRepository;

    public GetBookByIdHandler(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<Book?>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book =  await _bookRepository.GetBookByIdAsync(request.IdBook);
        if (book == null)
        {
            return Result<Book?>.Failure(ErrorType.NotFound, "Không tìm thấy sách với Id đã cho");
        }
        return Result<Book?>.Success(book);
    }
       
}