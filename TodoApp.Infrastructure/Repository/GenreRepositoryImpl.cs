using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Repository
{
    public class GenreRepositoryImpl : IGenreRepository
    {
        private readonly TodoAppDbContext _context;

        public GenreRepositoryImpl(TodoAppDbContext context)
        {
            _context = context;
        }

        public async Task AddGenreAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
        }

        public async Task<Genre?> GetGenreByIdAsync(int idGenre)
        {
            return await _context.Genres.FindAsync(idGenre);
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGenreAsync(Genre genre)
        {
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }

        public async Task<Genre?> GetNameGenreAsync(string nameGenre)
        {
            var genre = await _context.Genres
                .FirstOrDefaultAsync(g => g.NameGenre == nameGenre);
            return genre;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
