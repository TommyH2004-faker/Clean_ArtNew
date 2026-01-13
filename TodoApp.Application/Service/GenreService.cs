using TodoApp.Application.Common;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Service
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<Result<Genre>> CreateGenreAsync(Genre genre)
        {
            var existingGenre = await _genreRepository.GetNameGenreAsync(genre.NameGenre);
            if (existingGenre != null)
            {
                return Result<Genre>.Failure(ErrorType.Conflict, "Genre with the same name already exists.");
            }
            await _genreRepository.AddGenreAsync(genre);
            return Result<Genre>.Success(genre);
        }

        public async Task<Result<bool>> DeleteGenreAsync(int idGenre)
        {
            var genre = await _genreRepository.GetGenreByIdAsync(idGenre);
            if (genre == null)
            {
                return Result<bool>.Failure(ErrorType.NotFound, "Genre not found.");
            }

            await _genreRepository.DeleteGenreAsync(genre);
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<Genre>>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllGenresAsync();
            return Result<IEnumerable<Genre>>.Success(genres);
        }

        public async Task<Result<Genre?>> GetGenreByIdAsync(int idGenre)
        {
            var genre = await _genreRepository.GetGenreByIdAsync(idGenre);
            if (genre == null)
            {
                return Result<Genre?>.Failure(ErrorType.NotFound, "Genre not found.");
            }

            return Result<Genre?>.Success(genre);
        }

        public async Task<Result<Genre>> UpdateGenreAsync(Genre genre)
        {
            var existingGenre = await _genreRepository.GetGenreByIdAsync(genre.IdGenre);
            if (existingGenre == null)
            {
                return Result<Genre>.Failure(ErrorType.NotFound, "Genre not found.");
            }

            await _genreRepository.UpdateGenreAsync(genre);
            return Result<Genre>.Success(genre);    
        }
    }
}