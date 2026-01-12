using MediatR;
using TodoApp.Application.Common;
using TodoApp.Domain.Entities;
namespace TodoApp.Application.Features.BookHandle.Command;

public record CreateBookCommand(
    string NameBook,
    string Author,
    string Description,
    int ListPrice,
    int Price,
    int Quantity
) : IRequest<Result<Book?>>;
