using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.GenreDTOs;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.GenreHandle.Command.Update
{
    public class UpdateGenreCommandHandler : IRequestHandler<UpdateGenreCommand, Result<GenreResponseDTO>>
    {
        private readonly IGenreRepository _genreRepository;

        public UpdateGenreCommandHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<Result<GenreResponseDTO>> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra Genre có tồn tại không
            var genre = await _genreRepository.GetGenreByIdAsync(request.IdGenre);
            if (genre == null)
            {
                return Result<GenreResponseDTO>.Failure(ErrorType.NotFound, "Không tìm thấy thể loại");
            }

            // Kiểm tra trùng tên (ngoại trừ chính nó)
            var existingGenre = await _genreRepository.GetNameGenreAsync(request.NameGenre);
            if (existingGenre != null && existingGenre.IdGenre != request.IdGenre)
            {
                return Result<GenreResponseDTO>.Failure(ErrorType.Conflict, "Tên thể loại đã tồn tại");
            }

            // Gọi Domain Method để update - Entity tự quản lý ID
            genre.Update(request.NameGenre);

            // Lưu vào database
            await _genreRepository.UpdateGenreAsync(genre);

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
