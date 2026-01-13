namespace TodoApp.Application.DTOs
{
    public class GenreResponseDTO
    {
        public int IdGenre { get; set; }
        public string NameGenre { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
