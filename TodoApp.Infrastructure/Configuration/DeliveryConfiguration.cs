using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.ToTable("Deliveries");
            
            // Primary Key
            builder.HasKey(d => d.IdDelivery);
            
            // Properties
            builder.Property(d => d.IdDelivery)
                .ValueGeneratedOnAdd();
            
            builder.Property(d => d.DeliveryAddress)
                .HasMaxLength(500)
                .IsRequired();
            
            builder.Property(d => d.ReceiverName)
                .HasMaxLength(200)
                .IsRequired();
            
            builder.Property(d => d.ReceiverPhone)
                .HasMaxLength(20)
                .IsRequired();
            
            builder.Property(d => d.Status)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(d => d.TrackingNumber)
                .HasMaxLength(100);
            
            builder.Property(d => d.Note)
                .HasMaxLength(500);
            
            // Relationships
            builder.HasOne(d => d.Order)
                .WithOne(o => o.Delivery)
                .HasForeignKey<Delivery>(d => d.IdOrder)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(d => d.IdOrder)
                .IsUnique();
            builder.HasIndex(d => d.TrackingNumber);
            builder.HasIndex(d => d.Status);
        }
    }
}
