
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Repository
{
    public interface IGenreRepository
    {
        Task AddGenreAsync(Genre genre);
        Task<Genre?> GetGenreByIdAsync(int idGenre);
        Task UpdateGenreAsync(Genre genre);
        Task DeleteGenreAsync(Genre genre);
        Task<Genre?> GetNameGenreAsync(string nameGenre);
        Task<IEnumerable<Genre>> GetAllGenresAsync();
        
        /// <summary>
        /// Lưu các thay đổi vào database.
        /// Dùng để dispatch Domain Events sau khi entity đã có ID.
        /// </summary>
        Task SaveChangesAsync();
    }
}