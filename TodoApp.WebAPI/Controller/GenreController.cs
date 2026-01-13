using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Common;
using TodoApp.Application.Features.GenreHandle.Command.Create;
using TodoApp.Application.Features.GenreHandle.Command.Delete;
using TodoApp.Application.Features.GenreHandle.Command.Update;
using TodoApp.Application.Features.GenreHandle.Queries.GetAllGenres;
using TodoApp.Application.Features.GenreHandle.Queries.GetGenreById;

namespace TodoApp.WebAPI.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenreController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GenreController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ========== QUERIES (READ) ========== //

        /// <summary>
        /// Lấy danh sách tất cả thể loại
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var query = new GetAllGenresQuery();
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new
            {
                message = "Lấy danh sách thể loại thành công",
                data = result.Data,
                count = result.Data?.Count() ?? 0
            });
        }

        /// <summary>
        /// Lấy thông tin chi tiết 1 thể loại theo ID
        /// </summary>
        [HttpGet("{idGenre}")]
        public async Task<IActionResult> GetGenreById(int idGenre)
        {
            var query = new GetGenreByIdQuery { IdGenre = idGenre };
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(new
            {
                message = "Lấy thông tin thể loại thành công",
                data = result.Data
            });
        }

        // ========== COMMANDS (WRITE) ========== //

        /// <summary>
        /// Tạo thể loại mới (Chỉ Admin)
        /// </summary>
        [HttpPost]
 
        public async Task<IActionResult> CreateGenre([FromBody] CreateGenreCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });
            }

            //  Trả về 201 Created với Location header
            return CreatedAtAction(
                nameof(GetGenreById),
                new { idGenre = result.Data?.IdGenre },
                new
                {
                    message = "Tạo thể loại thành công",
                    data = result.Data
                }
            );
        }

        /// <summary>
        /// Cập nhật thể loại (Chỉ Admin)
        /// </summary>
        [HttpPut("{idGenre}")]
        public async Task<IActionResult> UpdateGenre(int idGenre, [FromBody] UpdateGenreCommand command)
        {
            // Đảm bảo ID từ route khớp với ID trong body
            if (idGenre != command.IdGenre)
            {
                return BadRequest(new { message = "ID thể loại không khớp" });
            }

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.ErrorType switch
                {
                    ErrorType.NotFound => NotFound(new { message = result.ErrorMessage }),
                    ErrorType.Conflict => Conflict(new { message = result.ErrorMessage }),
                    _ => BadRequest(new { message = result.ErrorMessage, errors = result.Errors })
                };
            }

            return Ok(new
            {
                message = "Cập nhật thể loại thành công",
                data = result.Data
            });
        }

        /// <summary>
        /// Xóa thể loại (Chỉ Admin)
        /// </summary>
        [HttpDelete("{idGenre}")]
        public async Task<IActionResult> DeleteGenre(int idGenre)
        {
            var command = new DeleteGenreCommand { IdGenre = idGenre };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return result.ErrorType == ErrorType.NotFound
                    ? NotFound(new { message = result.ErrorMessage })
                    : BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Xóa thể loại thành công" });
        }
    }
}
