using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repository
{
    public class BookRepositoryImpl : BookRepository
    {
        private readonly TodoAppDbContext _context;

        public BookRepositoryImpl(TodoAppDbContext context)
        {
            _context = context;
        }

        public async Task AddBookAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int idBook)
        {
            return await _context.Books.FindAsync(idBook);
        }

        public async Task UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetNameBookAsync(string nameBook)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.NameBook == nameBook);
        }
    }
}
