using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.Mappings;

namespace TodoApp.Application.Features.BookHandle.Queries.GetAllBooks
{
    public record GetAllBooksQuery : IRequest<Result<List<BookResponseDTO>>>
    {
        // Có thể thêm filter/pagination parameters sau
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? SearchTerm { get; set; }
    }
}
