namespace TodoApp.Application.DTOs;
public class CreateBookDto
{
    public string NameBook { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ListPrice { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
}