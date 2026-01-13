using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class OrdersConfiguration : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            builder.ToTable("Orders");
            
            // Primary Key
            builder.HasKey(o => o.IdOrder);
            
            // Properties
            builder.Property(o => o.IdOrder)
                .ValueGeneratedOnAdd();
            
            builder.Property(o => o.TotalPrice)
                .HasPrecision(18, 2)
                .IsRequired();
            
            builder.Property(o => o.Status)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(o => o.Note)
                .HasMaxLength(500);
            
            builder.Property(o => o.CreatedAt)
                .IsRequired();
            
            // Relationships
            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.IdUser)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.IdOrder)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.IdOrder)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(o => o.Delivery)
                .WithOne(d => d.Order)
                .HasForeignKey<Delivery>(d => d.IdOrder)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(o => o.IdUser);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.CreatedAt);
        }
    }
}
