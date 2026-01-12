using MediatR;
using TodoApp.Application.Common;

namespace TodoApp.Application.Features.BookHandle.Command
{
    public class DeleteBookCommand : IRequest<Result<bool>>
    {
        public int IdBook { get; set; }

        public DeleteBookCommand(int idBook)
        {
            IdBook = idBook;
        }
    }
}
