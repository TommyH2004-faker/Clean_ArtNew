using TodoApp.Domain.Common;
using TodoApp.Domain.Events;

namespace TodoApp.Domain.Entities
{
    public class Genre 
    {
        public int IdGenre { get; private set; }
        public string NameGenre { get; private set; } = null!;
        public DateTime? CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        public ICollection<BookGenre> BookGenres { get; private set; }
        = new List<BookGenre>();

        private Genre() { }
        
        // Factory Method - DDD Pattern
        public static Genre Create(string nameGenre)
        {
            if (string.IsNullOrWhiteSpace(nameGenre))
                throw new ArgumentException("NameGenre cannot be empty", nameof(nameGenre));
    
            var genre = new Genre
            {
                NameGenre = nameGenre,
                CreatedAt = DateTime.UtcNow
            };

            return genre;
        }

   
        public void Update(string nameGenre)
        {
            if (string.IsNullOrWhiteSpace(nameGenre))
                throw new ArgumentException("NameGenre cannot be empty", nameof(nameGenre));

            this.NameGenre = nameGenre;
            this.UpdatedAt = DateTime.UtcNow;
        }
        public void RemoveBookGenre(int bookId)
        {
            var bookGenre = this.BookGenres.FirstOrDefault(bg => bg.BookId == bookId);
            if (bookGenre == null)
                throw new InvalidOperationException($"Book {bookId} is not associated with this genre");

            this.BookGenres.Remove(bookGenre);
        }

      
    }
}