namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity: CartItem (Giỏ hàng)
    /// Aggregate Root hoặc Entity thuộc Cart aggregate
    /// </summary>
    public class CartItem
    {
        public int IdCartItem { get; private set; }
        public int IdUser { get; private set; }
        public int IdBook { get; private set; }
        public int Quantity { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation properties
        public User User { get; private set; } = null!;
        public Book Book { get; private set; } = null!;

        private CartItem() { }

        /// <summary>
        /// Factory method: Tạo CartItem mới
        /// </summary>
        public static CartItem Create(int idUser, int idBook, int quantity = 1)
        {
            if (idUser <= 0)
                throw new ArgumentException("User ID must be positive", nameof(idUser));

            if (idBook <= 0)
                throw new ArgumentException("Book ID must be positive", nameof(idBook));

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive");

            return new CartItem
            {
                IdUser = idUser,
                IdBook = idBook,
                Quantity = quantity,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Business logic: Cập nhật số lượng
        /// </summary>
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be positive");

            Quantity = newQuantity;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Tăng số lượng
        /// </summary>
        public void IncreaseQuantity(int amount = 1)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");

            Quantity += amount;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Giảm số lượng
        /// </summary>
        public void DecreaseQuantity(int amount = 1)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");

            if (Quantity - amount <= 0)
                throw new InvalidOperationException("Quantity cannot be zero or negative. Remove the item instead.");

            Quantity -= amount;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Validation: Kiểm tra trước khi xóa
        /// </summary>
        public void ValidateForDeletion()
        {
            // Có thể thêm business rules nếu cần
        }
    }
}
