using TodoApp.Domain.Entities;

namespace TodoApp.Application.Repository
{
    public interface BookRepository
    {
        Task <Book?> AddBookAsync(Book book);
        Task<Book?> GetBookByIdAsync(int idBook);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(Book book);
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetNameBookAsync(string nameBook);

    }
}