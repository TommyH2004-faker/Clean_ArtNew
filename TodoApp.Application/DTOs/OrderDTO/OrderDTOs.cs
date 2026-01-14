namespace TodoApp.Application.DTOs.OrderDTO
{
    public class OrderResponseDTO
    {
        public int IdOrder { get; set; }
        public int IdUser { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; } = new();
    }

    public class OrderDetailDTO
    {
        public int IdOrderDetail { get; set; }
        public int IdBook { get; set; }
        public string? BookName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CreateOrderItemDTO
    {
        public int IdBook { get; set; }
        public int Quantity { get; set; }
    }
}
