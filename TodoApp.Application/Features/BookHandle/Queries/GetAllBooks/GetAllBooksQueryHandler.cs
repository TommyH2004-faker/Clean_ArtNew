using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.Mappings;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.BookHandle.Queries.GetAllBooks
{
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, Result<List<BookResponseDTO>>>
    {
        private readonly BookRepository _bookRepository;

        public GetAllBooksQueryHandler(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Result<List<BookResponseDTO>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            // Lấy tất cả books từ repository
            var books = await _bookRepository.GetAllBooksAsync();

            // Áp dụng filter nếu có search term
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                books = books.Where(b => 
                    b.NameBook.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Áp dụng pagination nếu có
            if (request.PageNumber.HasValue && request.PageSize.HasValue)
            {
                books = books
                    .Skip((request.PageNumber.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value)
                    .ToList();
            }

            // Map sang DTO
            var bookDTOs = books.Select(book => new BookResponseDTO
            {
                IdBook = book.IdBook,
                Author = book.Author,
                AvgRating = book.AvgRating,
                Description = book.Description,
                ListPrice = book.ListPrice,
                NameBook = book.NameBook,
                Quantity = book.Quantity,
                SellPrice = book.SellPrice,
                SoldQuantity = book.SoldQuantity,
                DiscountPercent = book.DiscountPercent,
                UrlImage = book.UrlImage,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt
            }).ToList();

            return Result<List<BookResponseDTO>>.Success(bookDTOs);
        }
    }
}
