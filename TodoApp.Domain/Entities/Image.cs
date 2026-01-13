namespace TodoApp.Domain.Entities
{
    /// <summary>
    /// Entity: Image (Hình ảnh)
    /// Có thể dùng cho Book, User, hoặc các entity khác
    /// Hỗ trợ multiple images và ordering
    /// </summary>
    public class Image
    {
        public int IdImage { get; private set; }
        public int IdBook { get; private set; }
        public string Url { get; private set; } = null!;
        public string? AltText { get; private set; }
        public bool IsPrimary { get; private set; } // Ảnh chính
        public int DisplayOrder { get; private set; } // Thứ tự hiển thị
        public long? FileSize { get; private set; } // Size in bytes
        public string? ContentType { get; private set; } // image/jpeg, image/png, etc.
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Navigation properties
        public Book Book { get; private set; } = null!;

        private Image() { }

        /// <summary>
        /// Factory method: Tạo Image mới cho Book
        /// </summary>
        public static Image CreateForBook(int idBook, string url, string? altText = null, 
            bool isPrimary = false, int displayOrder = 0, long? fileSize = null, string? contentType = null)
        {
            if (idBook <= 0)
                throw new ArgumentException("Book ID must be positive", nameof(idBook));

            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL cannot be empty", nameof(url));

            if (!IsValidUrl(url))
                throw new ArgumentException("Invalid image URL format", nameof(url));

            if (displayOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(displayOrder), "Display order cannot be negative");

            if (fileSize.HasValue && fileSize.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(fileSize), "File size must be positive");

            return new Image
            {
                IdBook = idBook,
                Url = url,
                AltText = altText ?? "Book image",
                IsPrimary = isPrimary,
                DisplayOrder = displayOrder,
                FileSize = fileSize,
                ContentType = contentType ?? "image/jpeg",
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Business logic: Đặt làm ảnh chính
        /// </summary>
        public void SetAsPrimary()
        {
            IsPrimary = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Bỏ đặt ảnh chính
        /// </summary>
        public void UnsetPrimary()
        {
            IsPrimary = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Cập nhật URL
        /// </summary>
        public void UpdateUrl(string newUrl)
        {
            if (string.IsNullOrWhiteSpace(newUrl))
                throw new ArgumentException("Image URL cannot be empty", nameof(newUrl));

            if (!IsValidUrl(newUrl))
                throw new ArgumentException("Invalid image URL format", nameof(newUrl));

            Url = newUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Cập nhật alt text
        /// </summary>
        public void UpdateAltText(string altText)
        {
            if (string.IsNullOrWhiteSpace(altText))
                throw new ArgumentException("Alt text cannot be empty", nameof(altText));

            AltText = altText;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Cập nhật thứ tự hiển thị
        /// </summary>
        public void UpdateDisplayOrder(int order)
        {
            if (order < 0)
                throw new ArgumentOutOfRangeException(nameof(order), "Display order cannot be negative");

            DisplayOrder = order;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Business logic: Cập nhật metadata
        /// </summary>
        public void UpdateMetadata(long? fileSize, string? contentType)
        {
            if (fileSize.HasValue && fileSize.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(fileSize), "File size must be positive");

            FileSize = fileSize;
            ContentType = contentType;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Validation: Kiểm tra URL hợp lệ
        /// </summary>
        private static bool IsValidUrl(string url)
        {
            // Accept both absolute URLs and relative paths
            if (url.StartsWith("/") || url.StartsWith("./") || url.StartsWith("../"))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Business logic: Kiểm tra xem có phải ảnh hợp lệ không
        /// </summary>
        public bool IsValidImageType()
        {
            if (string.IsNullOrWhiteSpace(ContentType))
                return false;

            var validTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/bmp" };
            return validTypes.Contains(ContentType.ToLower());
        }

        /// <summary>
        /// Business logic: Kiểm tra file size có quá lớn không
        /// </summary>
        public bool IsFileSizeExceeded(long maxSizeInBytes = 5242880) // Default: 5MB
        {
            return FileSize.HasValue && FileSize.Value > maxSizeInBytes;
        }
    }
}
