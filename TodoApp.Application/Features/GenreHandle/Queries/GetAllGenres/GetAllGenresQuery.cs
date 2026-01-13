using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Features.GenreHandle.Queries.GetAllGenres
{
    public class GetAllGenresQuery : IRequest<Result<IEnumerable<GenreResponseDTO>>>
    {
    }
}
