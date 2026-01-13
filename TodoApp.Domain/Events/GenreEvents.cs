using TodoApp.Domain.Common;

namespace TodoApp.Domain.Events
{
    /// <summary>
    /// Domain Events cho Genre - Pure domain data contracts
    /// Không phụ thuộc MediatR hay infrastructure
    /// </summary>
    public static class GenreEvents
    {
        /// <summary>
        /// Genre mới được tạo
        /// </summary>
        public record GenreCreated : DomainEventBase
        {
            public int GenreId { get; init; }
            public string GenreName { get; init; }

            public GenreCreated(int genreId, string genreName)
            {
                GenreId = genreId;
                GenreName = genreName;
            }
        }

        /// <summary>
        /// Genre được cập nhật
        /// </summary>
        public record GenreUpdated : DomainEventBase
        {
            public int GenreId { get; init; }
            public string OldName { get; init; }
            public string NewName { get; init; }

            public GenreUpdated(int genreId, string oldName, string newName)
            {
                GenreId = genreId;
                OldName = oldName;
                NewName = newName;
            }
        }

        /// <summary>
        /// Genre bị xóa
        /// </summary>
        public record GenreDeleted : DomainEventBase
        {
            public int GenreId { get; init; }
            public string GenreName { get; init; }

            public GenreDeleted(int genreId, string genreName)
            {
                GenreId = genreId;
                GenreName = genreName;
            }
        }
    }
}
