using MediatR;
using TodoApp.Application.Common;

namespace TodoApp.Application.Features.GenreHandle.Command.Delete
{
    public class DeleteGenreCommand : IRequest<Result<bool>>
    {
        public int IdGenre { get; set; }
    }
}
