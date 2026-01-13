namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity: Review (Đánh giá sách)
    /// Entity hoặc có thể là aggregate root nếu có business logic phức tạp
    /// </summary>
    public class Review
    {
        public int IdReview { get; private set; }
        public int IdUser { get; private set; }
        public int IdBook { get; private set; }
        public int Rating { get; private set; } // 1-5 stars
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsVerifiedPurchase { get; private set; } // User đã mua sách chưa

        // Navigation properties
        public User User { get; private set; } = null!;
        public Book Book { get; private set; } = null!;

        private Review() { }

        /// <summary>
        /// Factory method: Tạo Review mới
        /// </summary>
        public static Review Create(int idUser, int idBook, int rating, string? comment = null, bool isVerifiedPurchase = false)
        {
            if (idUser <= 0)
                throw new ArgumentException("User ID must be positive", nameof(idUser));

            if (idBook <= 0)
                throw new ArgumentException("Book ID must be positive", nameof(idBook));

            if (rating < 1 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

            return new Review
            {
                IdUser = idUser,
                IdBook = idBook,
                Rating = rating,
                Comment = comment,
                IsVerifiedPurchase = isVerifiedPurchase,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Business logic: Cập nhật review
        /// </summary>
        public void Update(int rating, string? comment)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

            Rating = rating;
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Đánh dấu verified purchase
        /// </summary>
        public void MarkAsVerifiedPurchase()
        {
            IsVerifiedPurchase = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Validation: Kiểm tra trước khi xóa
        /// </summary>
        public void ValidateForDeletion(int userId)
        {
            if (IdUser != userId)
                throw new UnauthorizedAccessException("Only the review owner can delete this review");
        }
    }
}
