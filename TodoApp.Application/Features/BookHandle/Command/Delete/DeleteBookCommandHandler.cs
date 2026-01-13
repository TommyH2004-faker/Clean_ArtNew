using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.BookHandle.Command
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Result<bool>>
    {
        private readonly IBookRepository _bookRepository;

        public DeleteBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Result<bool>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            // Tìm book theo ID
            var book = await _bookRepository.GetBookByIdAsync(request.IdBook);
            if (book == null)
            {
                return Result<bool>.Failure(ErrorType.NotFound, "Book not found");
            }

            // Gọi Domain method để validate business rules (nếu có)
            book.DeleteBook(request.IdBook);

            // Xóa khỏi database
            await _bookRepository.DeleteBookAsync(book);

            return Result<bool>.Success(true);
        }
    }
}
