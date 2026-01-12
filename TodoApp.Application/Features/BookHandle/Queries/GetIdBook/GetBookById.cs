using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.Mappings;

namespace TodoApp.Application.Features.BookHandle.Queries.GetIdBook;

public record GetBookByIdQuery(int IdBook) : IRequest<Result<BookResponseDTO>>;