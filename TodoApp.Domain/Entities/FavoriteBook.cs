namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity: FavoriteBook (Sách yêu thích / Wishlist)
    /// Value Object or Entity - Quan hệ nhiều-nhiều giữa User và Book
    /// </summary>
    public class FavoriteBook
    {
        public int IdFavorite { get; private set; }
        public int IdUser { get; private set; }
        public int IdBook { get; private set; }
        public DateTime AddedAt { get; private set; }

        // Navigation properties
        public User User { get; private set; } = null!;
        public Book Book { get; private set; } = null!;

        private FavoriteBook() { }

        /// <summary>
        /// Factory method: Thêm sách vào danh sách yêu thích
        /// </summary>
        public static FavoriteBook Create(int idUser, int idBook)
        {
            if (idUser <= 0)
                throw new ArgumentException("User ID must be positive", nameof(idUser));

            if (idBook <= 0)
                throw new ArgumentException("Book ID must be positive", nameof(idBook));

            return new FavoriteBook
            {
                IdUser = idUser,
                IdBook = idBook,
                AddedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Validation: Kiểm tra trước khi xóa
        /// </summary>
        public void ValidateForDeletion(int userId)
        {
            if (IdUser != userId)
                throw new UnauthorizedAccessException("Only the owner can remove this favorite");
        }
    }
}
