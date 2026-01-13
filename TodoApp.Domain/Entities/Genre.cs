using TodoApp.Domain.Common;
using TodoApp.Domain.Events;

namespace TodoApp.Domain.Entities
{
    public class Genre : IHasDomainEvents
    {
        public int IdGenre { get; private set; }
        public string NameGenre { get; private set; } = null!;
        public DateTime? CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        public ICollection<BookGenre> BookGenres { get; private set; }
        = new List<BookGenre>();

        // Domain Events Support
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

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

            // KHÔNG raise event ở đây vì IdGenre = 0
            // Event sẽ được raise sau khi SaveChanges qua RaiseCreatedEvent()

            return genre;
        }

        /// <summary>
        /// Raise Created event SAU khi entity đã được save vào DB.
        /// Gọi method này trong Handler sau khi SaveChanges thành công.
        /// Lúc này IdGenre đã có giá trị thật từ database.
        /// </summary>
        public void RaiseCreatedEvent()
        {
            AddDomainEvent(new GenreEvents.GenreCreated(this.IdGenre, this.NameGenre));
        }
        
        // Domain Method - Update
        public void Update(string nameGenre)
        {
            if (string.IsNullOrWhiteSpace(nameGenre))
                throw new ArgumentException("NameGenre cannot be empty", nameof(nameGenre));

            var oldName = this.NameGenre;
            this.NameGenre = nameGenre;
            this.UpdatedAt = DateTime.UtcNow;

            // Raise Domain Event
            AddDomainEvent(new GenreEvents.GenreUpdated(this.IdGenre, oldName, nameGenre));
        }

        // Domain Method - Business Validation cho Delete
        public bool CanBeDeleted()
        {
            return !this.BookGenres.Any();
        }

        public void ValidateForDeletion()
        {
            if (!CanBeDeleted())
            {
                throw new InvalidOperationException(
                    $"Cannot delete genre '{NameGenre}' because it has {BookGenres.Count} book(s) associated. Remove all books first.");
            }
        }

        // Aggregate Root - Quản lý BookGenres collection
        public void AddBookGenre(int bookId)
        {
            if (bookId <= 0)
                throw new ArgumentException("BookId must be greater than 0", nameof(bookId));

            if (this.BookGenres.Any(bg => bg.BookId == bookId))
                throw new InvalidOperationException($"Book {bookId} is already associated with this genre");

            // Sử dụng public constructor của BookGenre
            var bookGenre = new BookGenre(bookId, this.IdGenre);
            
            this.BookGenres.Add(bookGenre);
        }

        public void RemoveBookGenre(int bookId)
        {
            var bookGenre = this.BookGenres.FirstOrDefault(bg => bg.BookId == bookId);
            if (bookGenre == null)
                throw new InvalidOperationException($"Book {bookId} is not associated with this genre");

            this.BookGenres.Remove(bookGenre);
        }

        // Domain Method - Mark for deletion (raises event)
        public void MarkForDeletion()
        {
            ValidateForDeletion();
            
            // Raise Domain Event
            AddDomainEvent(new GenreEvents.GenreDeleted(this.IdGenre, this.NameGenre));
        }

        // Domain Events Management
        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}