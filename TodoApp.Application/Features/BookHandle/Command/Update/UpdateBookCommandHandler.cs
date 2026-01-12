using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.Features.BookHandle.Command.Update;
using TodoApp.Application.Mappings;
using TodoApp.Application.Repository;

namespace TodoApp.Application.Features.BookHandle.Command
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Result<BookResponseDTO>>
    {
        private readonly BookRepository _bookRepository;

        public UpdateBookCommandHandler(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Result<BookResponseDTO>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            // Tìm book theo ID
            var existingBook = await _bookRepository.GetBookByIdAsync(request.IdBook);
            if (existingBook == null)
            {
                return Result<BookResponseDTO>.Failure(ErrorType.NotFound, "Không tìm thấy sách");
            }

            // ✅ Sử dụng Domain method Update để đảm bảo business rules
            existingBook.Update(
                IdBook: request.IdBook,
                author: request.Author,
                nameBook: request.NameBook,
                description: request.Description,
                urlImage: request.UrlImage,
                listPrice: request.ListPrice,
                sellPrice: request.SellPrice,
                quantity: request.Quantity,
                avgRating: request.AvgRating,
                discountPercent: request.DiscountPercent
                );
        
            // Lưu vào database
            await _bookRepository.UpdateBookAsync(existingBook);

            // ✅ Mapping Domain → DTO để trả về thông tin sách đã cập nhật
            var bookDTO = new BookResponseDTO
            {
                IdBook = existingBook.IdBook,
                Author = existingBook.Author,
                AvgRating = existingBook.AvgRating,
                Description = existingBook.Description,
                ListPrice = existingBook.ListPrice,
                NameBook = existingBook.NameBook,
                Quantity = existingBook.Quantity,
                SellPrice = existingBook.SellPrice,
                SoldQuantity = existingBook.SoldQuantity,
                DiscountPercent = existingBook.DiscountPercent,
                UrlImage = existingBook.UrlImage,
                CreatedAt = existingBook.CreatedAt,
                UpdatedAt = existingBook.UpdatedAt
            };

            return Result<BookResponseDTO>.Success(bookDTO);
        }
    }
}
