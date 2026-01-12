using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.Features.BookHandle.Command;
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

    // CREATE
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage, errors = result.Errors });

        return Created(
            $"api/books/{result.Data}",
            new { message = "Tạo sách thành công", bookId = result.Data }
        );
    }
    [HttpGet("{idBook}")]
    public async Task<IActionResult> GetBookById(int idBook)
    {
        var query = new GetBookByIdQuery(idBook);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Data);
    }


    
}

}