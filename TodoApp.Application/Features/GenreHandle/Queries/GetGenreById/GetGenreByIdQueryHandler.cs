using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.GenreDTOs;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.GenreHandle.Queries.GetGenreById
{
    public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, Result<GenreResponseDTO>>
    {
        private readonly IGenreRepository _genreRepository;

        public GetGenreByIdQueryHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<Result<GenreResponseDTO>> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
        {
            var genre = await _genreRepository.GetGenreByIdAsync(request.IdGenre);

            if (genre == null)
            {
                return Result<GenreResponseDTO>.Failure(ErrorType.NotFound, "Không tìm thấy thể loại");
            }

            // Mapping Domain → DTO
            var genreDTO = new GenreResponseDTO
            {
                IdGenre = genre.IdGenre,
                NameGenre = genre.NameGenre,
                CreatedAt = genre.CreatedAt,
                UpdatedAt = genre.UpdatedAt
            };

            return Result<GenreResponseDTO>.Success(genreDTO);
        }
    }
}
