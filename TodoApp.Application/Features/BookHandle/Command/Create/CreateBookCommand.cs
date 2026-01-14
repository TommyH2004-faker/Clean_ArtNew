using MediatR;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs.BookDTOs;

namespace TodoApp.Application.Features.BookHandle.Command;

/// <summary>
/// Command để tạo Book mới
/// Không kế thừa DTO - Command và DTO là 2 concerns khác nhau
/// </summary>
public class CreateBookCommand : IRequest<Result<BookResponseDTO>>
{
    public string NameBook { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ListPrice { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
}
