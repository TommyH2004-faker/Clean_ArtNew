using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.GenreDTOs;


namespace TodoApp.Application.Features.GenreHandle.Command.Create
{
    public class CreateGenreCommand : IRequest<Result<GenreResponseDTO>>
    {
        public string NameGenre { get; set; } = null!;
    }
}
