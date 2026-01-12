using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.IdBook);

            builder.Property(b => b.Author)
                   .HasMaxLength(200);

            builder.Property(b => b.NameBook)
                   .HasMaxLength(200);

            builder.Property(b => b.Description);

            builder.Property(b => b.UrlImage)
                   .HasMaxLength(500);

            builder.Property(b => b.ListPrice);
            builder.Property(b => b.SellPrice);
            builder.Property(b => b.Quantity);
            builder.Property(b => b.AvgRating);
    
            builder.Property(b => b.SoldQuantity);
               
            builder.Property(b => b.DiscountPercent);
      

            builder.Property(b => b.CreatedAt)
            .IsRequired(false);
            builder.Property(b => b.UpdatedAt)
            .IsRequired(false);
            builder.Property(b => b.IdBook)
                   .ValueGeneratedOnAdd();
        }
    }
}