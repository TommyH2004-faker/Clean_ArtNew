using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedbacks");
            
            // Primary Key
            builder.HasKey(f => f.IdFeedback);
            
            // Properties
            builder.Property(f => f.IdFeedback)
                .ValueGeneratedOnAdd();
            
            builder.Property(f => f.Subject)
                .HasMaxLength(200)
                .IsRequired();
            
            builder.Property(f => f.Message)
                .HasMaxLength(2000)
                .IsRequired();
            
            builder.Property(f => f.Status)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(f => f.AdminResponse)
                .HasMaxLength(2000);
            
            builder.Property(f => f.CreatedAt)
                .IsRequired();
            
            // Relationships
            builder.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.IdUser)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(f => f.IdUser);
            builder.HasIndex(f => f.Status);
            builder.HasIndex(f => f.CreatedAt);
        }
    }
}
