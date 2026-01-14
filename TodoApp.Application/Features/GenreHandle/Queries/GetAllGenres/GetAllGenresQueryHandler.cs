using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.GenreDTOs;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.GenreHandle.Queries.GetAllGenres
{
    public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, Result<IEnumerable<GenreResponseDTO>>>
    {
        private readonly IGenreRepository _genreRepository;

        public GetAllGenresQueryHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<Result<IEnumerable<GenreResponseDTO>>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
        {
            var genres = await _genreRepository.GetAllGenresAsync();

            // Mapping Domain → DTO
            var genreDTOs = genres.Select(g => new GenreResponseDTO
            {
                IdGenre = g.IdGenre,
                NameGenre = g.NameGenre,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            });

            return Result<IEnumerable<GenreResponseDTO>>.Success(genreDTOs);
        }
    }
}
