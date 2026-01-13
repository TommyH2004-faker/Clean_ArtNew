using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.BookHandle.Command;

public class CreateBookHandle : IRequestHandler<CreateBookCommand, Result<BookResponseDTO>>
{
    private readonly IBookRepository _bookRepository;
    public CreateBookHandle(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    public async Task<Result<BookResponseDTO>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra tên sách đã tồn tại chưa
        var existNameBook = await _bookRepository.GetNameBookAsync(request.NameBook);
        if (existNameBook != null)
        {
            return Result<BookResponseDTO>.Failure(ErrorType.Conflict, "Tên sách đã tồn tại trong hệ thống");
        }

        // Tạo Book bằng Domain Factory Method
        var newBook = Book.Create(
            author: request.Author,
            nameBook: request.NameBook,
            description: request.Description,
            quantity: request.Quantity,
            listPrice: request.ListPrice
        );
        
        // Lưu vào database - EF Core tự động set IdBook
        await _bookRepository.AddBookAsync(newBook);

        //  Mapping Domain → DTO để trả về thông tin sách vừa tạo
        var bookDTO = new BookResponseDTO
        {
            IdBook = newBook.IdBook,
            Author = newBook.Author,
            AvgRating = newBook.AvgRating,
            Description = newBook.Description,
            ListPrice = newBook.ListPrice,
            NameBook = newBook.NameBook,
            Quantity = newBook.Quantity,
            SellPrice = newBook.SellPrice,
            SoldQuantity = newBook.SoldQuantity,
            DiscountPercent = newBook.DiscountPercent,
            UrlImage = newBook.UrlImage,
            CreatedAt = newBook.CreatedAt,
            UpdatedAt = newBook.UpdatedAt
        };

        return Result<BookResponseDTO>.Success(bookDTO);
    }
}