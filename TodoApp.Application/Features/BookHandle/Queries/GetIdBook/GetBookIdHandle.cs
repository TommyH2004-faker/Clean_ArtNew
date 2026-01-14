using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.BookDTOs;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.BookHandle.Queries.GetIdBook;

public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, Result<BookResponseDTO>>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<BookResponseDTO>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetBookByIdAsync(request.IdBook);
        if (book == null)
        {
            return Result<BookResponseDTO>.Failure(ErrorType.NotFound, "Không tìm thấy sách với Id đã cho");
        }

        // ✅ Mapping Domain → DTO (giống GetAllBooks)
        var bookDTO = new BookResponseDTO
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
        };

        return Result<BookResponseDTO>.Success(bookDTO);
    }
}