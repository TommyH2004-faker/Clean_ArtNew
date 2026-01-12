using TodoApp.Application.Common;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Service
{
    public interface IGenreService
    {
        Task<Result<Genre>> CreateGenreAsync(Genre genre);
        Task<Result<Genre?>> GetGenreByIdAsync(int idGenre);
        Task<Result<IEnumerable<Genre>>> GetAllGenresAsync();
        Task<Result<Genre>> UpdateGenreAsync(Genre genre);
        Task<Result<bool>> DeleteGenreAsync(int idGenre);
    }

}
