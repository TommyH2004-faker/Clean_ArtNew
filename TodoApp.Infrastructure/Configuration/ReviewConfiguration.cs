using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            
            // Primary Key
            builder.HasKey(r => r.IdReview);
            
            // Properties
            builder.Property(r => r.IdReview)
                .ValueGeneratedOnAdd();
            
            builder.Property(r => r.Rating)
                .IsRequired();
            
            builder.Property(r => r.Comment)
                .HasMaxLength(1000);
            
            builder.Property(r => r.CreatedAt)
                .IsRequired();
            
            builder.Property(r => r.IsVerifiedPurchase)
                .IsRequired()
                .HasDefaultValue(false);
            
            // Relationships
            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.IdUser)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.IdBook)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(r => r.IdBook);
            builder.HasIndex(r => r.IdUser);
            builder.HasIndex(r => new { r.IdUser, r.IdBook })
                .IsUnique(); // Mỗi user chỉ review 1 lần cho 1 book
        }
    }
}
