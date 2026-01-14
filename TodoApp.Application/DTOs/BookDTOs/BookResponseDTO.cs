namespace TodoApp.Application.DTOs.BookDTOs
{
    public class BookResponseDTO
    {
        public int IdBook { get; set; }
        public string Author { get; set; } = string.Empty;
        public double AvgRating { get; set; }
        public string Description { get; set; } = string.Empty;
        public double ListPrice { get; set; }
        public string NameBook { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double SellPrice { get; set; }
        public int SoldQuantity { get; set; }
        public int DiscountPercent { get; set; }
        public string? UrlImage { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
