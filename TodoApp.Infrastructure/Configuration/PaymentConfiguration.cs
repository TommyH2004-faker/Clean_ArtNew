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
            
            builder.Property(p => p.Status)
                .HasMaxLength(50)
                .IsRequired();
            
      
            builder.HasIndex(p => p.Status);
        }
    }
}
