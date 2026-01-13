using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            
            // Primary Key
            builder.HasKey(p => p.IdPayment);
            
            // Properties
            builder.Property(p => p.IdPayment)
                .ValueGeneratedOnAdd();
            
            builder.Property(p => p.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();
            
            builder.Property(p => p.Status)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(p => p.TransactionId)
                .HasMaxLength(200);
            
            builder.Property(p => p.Note)
                .HasMaxLength(500);
            
            builder.Property(p => p.CreatedAt)
                .IsRequired();
            
            // Relationships
            builder.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.IdOrder)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(p => p.IdOrder)
                .IsUnique();
            builder.HasIndex(p => p.TransactionId);
            builder.HasIndex(p => p.Status);
        }
    }
}
