using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Features.BookHandle.Command;
using TodoApp.Application.Features.BookHandle.Command.Update;
using TodoApp.Application.Features.BookHandle.Queries.GetAllBooks;
using TodoApp.Application.Features.BookHandle.Queries.GetIdBook;

namespace TodoApp.WebAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ========== QUERIES (READ) ========== //

        /// <summary>
        /// Lấy danh sách tất cả sách (có filter + pagination)
        /// </summary>
        /// <remarks>
        /// Example: GET /api/books?searchTerm=harry&pageNumber=1&pageSize=10
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] GetAllBooksQuery query)
        {
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new
            {
                message = "Lấy danh sách sách thành công",
                data = result.Data,
                count = result.Data?.Count ?? 0
            });
        }

        /// <summary>
        /// Lấy thông tin chi tiết 1 cuốn sách theo ID
        /// </summary>
        [HttpGet("{idBook}")]
        public async Task<IActionResult> GetBookById(int idBook)
        {
            var query = new GetBookByIdQuery(idBook);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(new
            {
                message = "Lấy thông tin sách thành công",
                data = result.Data
            });
        }

        // ========== COMMANDS (WRITE) ========== //

        /// <summary>
        /// Tạo sách mới (Chỉ Admin)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });
            }
            
            // ✅ Trả về đầy đủ thông tin sách vừa tạo
            return CreatedAtAction(
                nameof(GetBookById),
                new { idBook = result.Data!.IdBook },
                new 
                { 
                    message = "Tạo sách thành công",
                    data = result.Data  // ✅ Thông tin đầy đủ của book
                }
            );
        }

        /// <summary>
        /// Cập nhật thông tin sách (Chỉ Admin)
        /// </summary>
        [HttpPut("{idBook}")]
        public async Task<IActionResult> UpdateBook(int idBook, [FromBody] UpdateBookCommand command)
        {
            // Đảm bảo idBook trong route match với command
            if (idBook != command.IdBook)
                return BadRequest(new { message = "Book ID mismatch" });

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.ErrorType == Application.Common.ErrorType.NotFound)
                    return NotFound(new { message = result.ErrorMessage });

                return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });
            }

            // ✅ Trả về đầy đủ thông tin sách đã cập nhật
            return Ok(new 
            { 
                message = "Cập nhật sách thành công",
                data = result.Data  // ✅ Thông tin book sau khi update
            });
        }

        /// <summary>
        /// Xóa sách (Chỉ Admin)
        /// </summary>
        [HttpDelete("{idBook}")]
      
        public async Task<IActionResult> DeleteBook(int idBook)
        {
            var command = new DeleteBookCommand(idBook);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.ErrorType == Application.Common.ErrorType.NotFound)
                    return NotFound(new { message = result.ErrorMessage });

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Xóa sách thành công" });
        }
    }
}