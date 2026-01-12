using TodoApp.Application.Common;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Service
{
    public class BookService : IBookService
    {
        private readonly BookRepository _bookRepository;

        public BookService(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Result<Book>> CreateBookAsync(Book book)
        {
            var existingBook = await _bookRepository.GetNameBookAsync(book.NameBook);
            if (existingBook != null)
            {
                return Result<Book>.Failure(ErrorType.Conflict, "Book with the same name already exists.");
            }
            await _bookRepository.AddBookAsync(book);
            return Result<Book>.Success(book);
        }

        public async Task<Result<bool>> DeleteBookAsync(int idBook)
        {
            var book = await _bookRepository.GetBookByIdAsync(idBook);
            if (book == null)
            {
                return Result<bool>.Failure(ErrorType.NotFound, "Book not found.");
            }

            await _bookRepository.DeleteBookAsync(book);
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<Book>>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllBooksAsync();
            return Result<IEnumerable<Book>>.Success(books);
        }

        public async Task<Result<Book?>> GetBookByIdAsync(int idBook)
        {
            var book = await _bookRepository.GetBookByIdAsync(idBook);
            if (book == null)
            {
                return Result<Book?>.Failure(ErrorType.NotFound, "Book not found.");
            }
            return Result<Book?>.Success(book);
        }

        public async Task<Result<Book>> UpdateBookAsync(Book book)
        {
            var existingBook = await _bookRepository.GetBookByIdAsync(book.IdBook);
            if (existingBook == null)
            {
                return Result<Book>.Failure(ErrorType.NotFound, "Book not found.");
            }

            await _bookRepository.UpdateBookAsync(book);
            return Result<Book>.Success(book);
        }
    }
}
