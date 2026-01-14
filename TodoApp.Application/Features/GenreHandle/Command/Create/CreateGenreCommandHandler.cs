using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.GenreDTOs;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.GenreHandle.Command.Create
{
    public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand, Result<GenreResponseDTO>>
    {
        private readonly IGenreRepository _genreRepository;

        public CreateGenreCommandHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<Result<GenreResponseDTO>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra tên thể loại đã tồn tại chưa
            var existingGenre = await _genreRepository.GetNameGenreAsync(request.NameGenre);
            if (existingGenre != null)
            {
                return Result<GenreResponseDTO>.Failure(ErrorType.Conflict, "Tên thể loại đã tồn tại trong hệ thống");
            }

            // Tạo Genre bằng Domain Factory Method
            var newGenre = Genre.Create(request.NameGenre);

            // Lưu vào database (sau khi save, IdGenre sẽ có giá trị thật)
            await _genreRepository.AddGenreAsync(newGenre);

            // Raise Domain Event SAU khi có ID thật từ database
            // Event sẽ được dispatch trong lần SaveChanges tiếp theo
            newGenre.RaiseCreatedEvent();
            await _genreRepository.SaveChangesAsync();

            // Mapping Domain → DTO
            var genreDTO = new GenreResponseDTO
            {
                IdGenre = newGenre.IdGenre,
                NameGenre = newGenre.NameGenre,
                CreatedAt = newGenre.CreatedAt,
                UpdatedAt = newGenre.UpdatedAt
            };

            return Result<GenreResponseDTO>.Success(genreDTO);
        }
    }
}
