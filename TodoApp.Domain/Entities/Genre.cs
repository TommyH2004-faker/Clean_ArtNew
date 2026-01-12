namespace TodoApp.Domain.Entities
{
    public class Genre
    {
        public int IdGenre { get; private set; }
        public string NameGenre { get; private set; } = null!;
        
        public ICollection<BookGenre> BookGenres { get; private set; }
        = new List<BookGenre>();
        private Genre() { }
        public static Genre CreateGenre(string nameGenre)
        {
            if (string.IsNullOrWhiteSpace(nameGenre))
                throw new ArgumentException("NameGenre cannot be empty", nameof(nameGenre));
    
            return new Genre
            {
                NameGenre = nameGenre
            };
        }
        public void UpdateGenre(string nameGenre)
        {
            if (string.IsNullOrWhiteSpace(nameGenre))
                throw new ArgumentException("NameGenre cannot be empty", nameof(nameGenre));
    
            this.NameGenre = nameGenre;
        }

        public void DeleteGenre(int idGenre)
        {
            if (this.IdGenre != idGenre)
            {
                throw new ArgumentException("Genre ID does not match.", nameof(idGenre));
            }
        }
    }
}