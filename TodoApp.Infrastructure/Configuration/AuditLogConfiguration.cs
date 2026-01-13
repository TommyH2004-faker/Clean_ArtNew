using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    /// <summary>
    /// EF Core Configuration cho AuditLog entity.
    /// </summary>
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.OldValues)
                .HasColumnType("TEXT");

            builder.Property(a => a.NewValues)
                .HasColumnType("TEXT");

            builder.Property(a => a.Timestamp)
                .IsRequired();

            builder.Property(a => a.PerformedBy)
                .HasMaxLength(100);

            // Index để query nhanh theo entity
            builder.HasIndex(a => new { a.EntityType, a.EntityId });

            // Index để query theo thời gian
            builder.HasIndex(a => a.Timestamp);
        }
    }
}
