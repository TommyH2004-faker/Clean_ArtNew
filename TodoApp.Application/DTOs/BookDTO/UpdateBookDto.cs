namespace TodoApp.Application.DTOs;
public class UpdateBookDto
{
    public int IdBook { get; set; }
    public string Author { get; set; } = string.Empty;
    public string NameBook { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UrlImage { get; set; } = string.Empty;
    public double ListPrice { get; set; }
    public double SellPrice { get; set; }
    public int Quantity { get; set; }
    public double AvgRating { get; set; }
    public int SoldQuantity { get; set; }
    public int DiscountPercent { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

}