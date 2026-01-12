using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Mappings;
public static class BookMappingDTO
{
    public static Book ToEntity( CreateBookDto dto)
    {
        return Book.Create(
            dto.Author,
            dto.NameBook,
            dto.Description,
            dto.ListPrice,
            dto.Quantity
        );
    }
}