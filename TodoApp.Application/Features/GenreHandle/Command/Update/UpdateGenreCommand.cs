using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.GenreDTOs;
namespace TodoApp.Application.Features.GenreHandle.Command.Update
{
    public class UpdateGenreCommand : IRequest<Result<GenreResponseDTO>>
    {
        public int IdGenre { get; set; }
        public string NameGenre { get; set; } = null!;
    }
}
