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
    
            return new Genre
            {
                NameGenre = nameGenre,
                CreatedAt = DateTime.UtcNow
            };
        }
        
        // Domain Method - Update
        public void Update(int idGenre, string nameGenre)
        {
            if (this.IdGenre != idGenre)
            {
                throw new ArgumentException("Genre ID does not match.", nameof(idGenre));
            }
            
            if (string.IsNullOrWhiteSpace(nameGenre))
                throw new ArgumentException("NameGenre cannot be empty", nameof(nameGenre));
    
            this.NameGenre = nameGenre;
            this.UpdatedAt = DateTime.UtcNow;
        }

        // Domain Method - Delete Validation
        public void Delete(int idGenre)
        {
            if (this.IdGenre != idGenre)
            {
                throw new ArgumentException("Genre ID does not match.", nameof(idGenre));
            }
        }
    }
}