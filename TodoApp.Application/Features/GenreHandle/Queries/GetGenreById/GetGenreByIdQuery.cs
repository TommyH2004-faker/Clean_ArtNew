using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Features.GenreHandle.Queries.GetGenreById
{
    public class GetGenreByIdQuery : IRequest<Result<GenreResponseDTO>>
    {
        public int IdGenre { get; set; }
    }
}
