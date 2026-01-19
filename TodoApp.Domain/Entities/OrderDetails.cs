namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity: OrderDetails (Chi tiết đơn hàng)
    /// Entity thuộc Orders aggregate
    /// </summary>
    public class OrderDetails
    {
        public int IdOrderDetail { get; private set; }
        public int IdOrder { get; private set; }
        public int IdBook { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; } // Giá tại thời điểm mua
        public decimal Subtotal { get; private set; }

        // Navigation properties
        public Orders Order { get; private set; } = null!;
        public Book Book { get; private set; } = null!;

        private OrderDetails() { }

        /// <summary>
        /// Factory method: Tạo OrderDetail mới
        /// NOTE: idOrder có thể = 0 khi tạo mới (EF Core sẽ tự động gán FK sau khi save Order)
        /// </summary>
        public static OrderDetails Create(int idOrder, int idBook, int quantity, decimal price)
        {
            // NOTE: Không validate idOrder > 0 vì khi tạo Order mới, IdOrder vẫn = 0
            // EF Core sẽ tự động gán IdOrder cho OrderDetails sau khi save Order
            
            if (idBook <= 0)
                throw new ArgumentException("Book ID must be positive", nameof(idBook));

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive");

            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");

            var orderDetail = new OrderDetails
            {
                IdOrder = idOrder,
                IdBook = idBook,
                Quantity = quantity,
                Price = price
            };

            orderDetail.CalculateSubtotal();
            return orderDetail;
        }

        /// <summary>
        /// Business logic: Tính subtotal
        /// </summary>
        public void CalculateSubtotal()
        {
            Subtotal = Quantity * Price;
        }

        /// <summary>
        /// Business logic: Cập nhật số lượng
        /// </summary>
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be positive");

            Quantity = newQuantity;
            CalculateSubtotal();
        }

        /// <summary>
        /// Business logic: Cập nhật giá
        /// </summary>
        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(newPrice), "Price cannot be negative");

            Price = newPrice;
            CalculateSubtotal();
        }
    }
}
