
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Repository
{
    public interface GenreRepository
    {
        Task AddGenreAsync(Genre genre);
        Task<Genre?> GetGenreByIdAsync(int idGenre);
        Task UpdateGenreAsync(Genre genre);
        Task DeleteGenreAsync(Genre genre);
        Task<Genre?> GetNameGenreAsync(string nameGenre);
        Task<IEnumerable<Genre>> GetAllGenresAsync();
    }
}