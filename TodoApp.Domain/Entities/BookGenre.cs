namespace TodoApp.Domain.Entities;
public class BookGenre
{
    public int BookId { get; private set; }
    public Book Book { get; private set; } = null!;

    public int GenreId { get; private set; }
    public Genre Genre { get; private set; } = null!;

    private BookGenre() { }

    public BookGenre(int bookId, int genreId)
    {
        BookId = bookId;
        GenreId = genreId;
    }
}

