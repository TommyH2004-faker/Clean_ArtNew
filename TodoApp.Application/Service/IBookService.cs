using TodoApp.Application.Common;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Service
{
    public interface IBookService
    {
        Task<Result<Book>> CreateBookAsync(Book book);
        Task<Result<Book?>> GetBookByIdAsync(int idBook);
        Task<Result<IEnumerable<Book>>> GetAllBooksAsync();
        Task<Result<Book>> UpdateBookAsync(Book book);
        Task<Result<bool>> DeleteBookAsync(int idBook);
    }
}
