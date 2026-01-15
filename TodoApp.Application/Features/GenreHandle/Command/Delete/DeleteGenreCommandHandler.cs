using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.GenreHandle.Command.Delete
{
    public class DeleteGenreCommandHandler : IRequestHandler<DeleteGenreCommand, Result<bool>>
    {
        private readonly IGenreRepository _genreRepository;

        public DeleteGenreCommandHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<Result<bool>> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
        {

            var genre = await _genreRepository.GetGenreByIdAsync(request.IdGenre);
            if (genre == null)
            {
                return Result<bool>.Failure(ErrorType.NotFound, "Không tìm thấy thể loại");
            }

            await _genreRepository.DeleteGenreAsync(genre);

            return Result<bool>.Success(true);
        }
    }
}
