using MediatR;
using TodoApp.Application.Common;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.BookHandle.Queries.GetIdBook;

public record GetBookByIdQuery(int IdBook) : IRequest<Result<Book?>>;