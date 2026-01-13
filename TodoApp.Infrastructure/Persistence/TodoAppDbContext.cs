using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Common;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Configuration;
using TodoApp.Infrastructure.Services;

namespace TodoApp.Infrastructure.Persistence
{
    public class TodoAppDbContext : DbContext
    {
        private readonly IDomainEventDispatcher _eventDispatcher;

        public TodoAppDbContext(DbContextOptions<TodoAppDbContext> options, IDomainEventDispatcher eventDispatcher) 
            : base(options)
        {
            _eventDispatcher = eventDispatcher;
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BookGenre> BookGenres { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        
        // New entities
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Orders> Orders { get; set; } = null!;
        public DbSet<OrderDetails> OrderDetails { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Delivery> Deliveries { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<FavoriteBook> FavoriteBooks { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new GenreConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new BookGenreConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
            
            // New entity configurations
            modelBuilder.ApplyConfiguration(new CartItemConfiguration());
            modelBuilder.ApplyConfiguration(new OrdersConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new DeliveryConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new FavoriteBookConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new ImageConfiguration());
        }

        /// <summary>
        /// Override SaveChangesAsync để dispatch Domain Events sau khi lưu thành công.
        /// 
        /// Event-Driven Architecture - Level 5
        /// 
        /// Luồng xử lý:
        /// 1. Lấy entities có Domain Events
        /// 2. Trích xuất events
        /// 3. Clear events khỏi entities
        /// 4. Save changes vào DB (đảm bảo data consistency)
        /// 5. Dispatch events qua DomainEventDispatcher (auto-discovery)
        /// 
        /// ✅ Auto-discovery: Không cần khai báo thủ công khi thêm event mới!
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

            // 4. Lưu changes vào database TRƯỚC
            var result = await base.SaveChangesAsync(cancellationToken);

            // 5. Dispatch events qua DomainEventDispatcher (tự động tìm wrapper)
            await _eventDispatcher.DispatchAllAsync(domainEvents, cancellationToken);

            return result;
        }
    }
}
