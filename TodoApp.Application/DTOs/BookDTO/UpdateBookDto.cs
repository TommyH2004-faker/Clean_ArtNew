namespace TodoApp.Application.DTOs;
public class UpdateBookDto
{
    //  public int IdBook { get; private set; }
    //     public string Author { get; private set; } = null!;
    //     public double AvgRating { get; private set; } 
    //     public string Description { get; private set; } = null!;
    //     public double ListPrice { get; private set; }
    //     public string NameBook { get; private set; } = null!;
    //     public int Quantity { get; private set; }
    //     public double SellPrice { get; private set; }
    //     public int SoldQuantity { get; private set; }
    //     public int DiscountPercent { get; private set; }
    //     public string UrlImage { get; private set; } = null!;
    //     public DateTime? CreatedAt { get; private set; }
    //     public DateTime? UpdatedAt { get; private set; }
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