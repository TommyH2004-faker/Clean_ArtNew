using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Common;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Configuration;
using GenreEvents = TodoApp.Domain.Events.GenreEvents;

namespace TodoApp.Infrastructure.Persistence
{
    public class TodoAppDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public TodoAppDbContext(DbContextOptions<TodoAppDbContext> options, IMediator mediator) 
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BookGenre> BookGenres { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new GenreConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new BookGenreConfiguration());
        }

        /// <summary>
        /// Override SaveChangesAsync để dispatch Domain Events sau khi lưu thành công
        /// Event-Driven Architecture - Level 5
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 1. Lấy tất cả entities có Domain Events
            var entitiesWithEvents = ChangeTracker.Entries<IHasDomainEvents>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            // 2. Lấy tất cả Domain Events trước khi save
            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // 3. Clear events khỏi entities (tránh dispatch lại)
            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            // 4. Lưu changes vào database
            var result = await base.SaveChangesAsync(cancellationToken);

            // 5. Convert Domain Events → MediatR Notifications và dispatch
            foreach (var domainEvent in domainEvents)
            {
                INotification? notification = domainEvent switch
                {
                    GenreEvents.GenreCreated e => new Application.Events.GenreCreatedEvent(e),
                    GenreEvents.GenreUpdated e => new Application.Events.GenreUpdatedEvent(e),
                    GenreEvents.GenreDeleted e => new Application.Events.GenreDeletedEvent(e),
                    _ => null
                };

                if (notification != null)
                {
                    await _mediator.Publish(notification, cancellationToken);
                }
            }

            return result;
        }
    }
}
