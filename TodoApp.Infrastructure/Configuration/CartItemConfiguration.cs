using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");
            
            // Primary Key
            builder.HasKey(c => c.IdCartItem);
            
            // Properties
            builder.Property(c => c.IdCartItem)
                .ValueGeneratedOnAdd();
            
            builder.Property(c => c.Quantity)
                .IsRequired();
            
            builder.Property(c => c.CreatedAt)
                .IsRequired();
            
            // Relationships
            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.IdUser)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(c => c.Book)
                .WithMany()
                .HasForeignKey(c => c.IdBook)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Index for better query performance
            builder.HasIndex(c => new { c.IdUser, c.IdBook })
                .IsUnique(); // Mỗi user chỉ có 1 entry cho 1 book
        }
    }
}
