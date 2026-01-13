using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
    {
        public void Configure(EntityTypeBuilder<OrderDetails> builder)
        {
            builder.ToTable("OrderDetails");
            
            // Primary Key
            builder.HasKey(od => od.IdOrderDetail);
            
            // Properties
            builder.Property(od => od.IdOrderDetail)
                .ValueGeneratedOnAdd();
            
            builder.Property(od => od.Quantity)
                .IsRequired();
            
            builder.Property(od => od.Price)
                .HasPrecision(18, 2)
                .IsRequired();
            
            builder.Property(od => od.Subtotal)
                .HasPrecision(18, 2)
                .IsRequired();
            
            // Relationships
            builder.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.IdOrder)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(od => od.Book)
                .WithMany()
                .HasForeignKey(od => od.IdBook)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indexes
            builder.HasIndex(od => od.IdOrder);
            builder.HasIndex(od => od.IdBook);
        }
    }
}
