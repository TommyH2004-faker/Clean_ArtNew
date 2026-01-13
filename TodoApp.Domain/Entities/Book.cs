
namespace TodoApp.Domain.Entities
{
    public class Book
    {
        public int IdBook { get; private set; }
        public string Author { get; private set; } = null!;
        public double AvgRating { get; private set; } 
        public string Description { get; private set; } = null!;
        public double ListPrice { get; private set; }
        public string NameBook { get; private set; } = null!;
        public int Quantity { get; private set; }
        public double SellPrice { get; private set; }
        public int SoldQuantity { get; private set; }
        public int DiscountPercent { get; private set; }
        public string? UrlImage { get; private set; } = null;
        public DateTime? CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public ICollection<BookGenre> BookGenres { get; private set; }= new List<BookGenre>();


        private Book() { }

        // Factory method
      
        public static Book Create(string author, string nameBook, string description , int quantity = 0 , int listPrice = 0)
        {
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author cannot be empty", nameof(author));

            if (string.IsNullOrWhiteSpace(nameBook))
                throw new ArgumentException("NameBook cannot be empty", nameof(nameBook));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));
            if (listPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(listPrice), "ListPrice cannot be negative.");

            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
            return new Book
            {
                Author = author,
                NameBook = nameBook,
                Description = description,
                ListPrice = listPrice,
                Quantity = quantity,
                CreatedAt = DateTime.UtcNow  
            };
        }

        public void Update(int IdBook , string author, string nameBook, string description, string urlImage,
            double listPrice, double sellPrice, int quantity, double avgRating, int discountPercent)
        {
            if( this.IdBook != IdBook)
            {
                throw new ArgumentException("Book ID does not match.", nameof(IdBook));
            }
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author cannot be empty", nameof(author));

            if (string.IsNullOrWhiteSpace(nameBook))
                throw new ArgumentException("NameBook cannot be empty", nameof(nameBook));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            if (string.IsNullOrWhiteSpace(urlImage))
                throw new ArgumentException("UrlImage cannot be empty", nameof(urlImage));

            if (avgRating < 0 || avgRating > 5)
                throw new ArgumentOutOfRangeException(nameof(avgRating), "AvgRating must be between 0 and 5.");

            if (listPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(listPrice), "ListPrice cannot be negative.");

            if (sellPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(sellPrice), "SellPrice cannot be negative.");

            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");

            if (discountPercent < 0 || discountPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercent), "DiscountPercent must be between 0 and 100.");

          
            Author = author;
            NameBook = nameBook;
            Description = description;
            UrlImage = urlImage;
            ListPrice = listPrice;
            SellPrice = sellPrice;
            Quantity = quantity;
            AvgRating = avgRating;
            DiscountPercent = discountPercent;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeleteBook(int IdBook){
            if(this.IdBook != IdBook)
            {
                throw new ArgumentException("Book ID does not match.", nameof(IdBook));
            }
        }
    }
}