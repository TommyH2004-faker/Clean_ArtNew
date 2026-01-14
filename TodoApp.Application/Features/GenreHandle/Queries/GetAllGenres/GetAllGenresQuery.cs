using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.GenreDTOs;

namespace TodoApp.Application.Features.GenreHandle.Queries.GetAllGenres
{
    public class GetAllGenresQuery : IRequest<Result<IEnumerable<GenreResponseDTO>>>
    {
    }
}
