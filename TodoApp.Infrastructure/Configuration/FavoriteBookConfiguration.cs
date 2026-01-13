using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class FavoriteBookConfiguration : IEntityTypeConfiguration<FavoriteBook>
    {
        public void Configure(EntityTypeBuilder<FavoriteBook> builder)
        {
            builder.ToTable("FavoriteBooks");
            
            // Primary Key
            builder.HasKey(f => f.IdFavorite);
            
            // Properties
            builder.Property(f => f.IdFavorite)
                .ValueGeneratedOnAdd();
            
            builder.Property(f => f.AddedAt)
                .IsRequired();
            
            // Relationships
            builder.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.IdUser)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(f => f.Book)
                .WithMany(b => b.FavoriteBooks)
                .HasForeignKey(f => f.IdBook)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(f => f.IdUser);
            builder.HasIndex(f => f.IdBook);
            builder.HasIndex(f => new { f.IdUser, f.IdBook })
                .IsUnique(); // Mỗi user chỉ favorite 1 lần cho 1 book
        }
    }
}
