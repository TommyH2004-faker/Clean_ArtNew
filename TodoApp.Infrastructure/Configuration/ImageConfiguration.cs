using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Images");
            
            // Primary Key
            builder.HasKey(i => i.IdImage);
            
            // Properties
            builder.Property(i => i.IdImage)
                .ValueGeneratedOnAdd();
            
            builder.Property(i => i.Url)
                .HasMaxLength(500)
                .IsRequired();
            
            builder.Property(i => i.AltText)
                .HasMaxLength(200);
            
            builder.Property(i => i.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);
            
            builder.Property(i => i.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);
            
            builder.Property(i => i.ContentType)
                .HasMaxLength(50);
            
            builder.Property(i => i.CreatedAt)
                .IsRequired();
            
            // Relationships
            builder.HasOne(i => i.Book)
                .WithMany(b => b.Images)
                .HasForeignKey(i => i.IdBook)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(i => i.IdBook);
            builder.HasIndex(i => new { i.IdBook, i.IsPrimary });
            builder.HasIndex(i => new { i.IdBook, i.DisplayOrder });
        }
    }
}
