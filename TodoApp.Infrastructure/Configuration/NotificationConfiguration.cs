using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    /// <summary>
    /// EF Core Configuration: Notification entity
    /// </summary>
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(n => n.IdNotification);

            builder.Property(n => n.IdNotification)
                .HasColumnName("id_notification")
                .ValueGeneratedOnAdd();

            builder.Property(n => n.Type)
                .HasColumnName("type")
                .HasConversion<int>() // Enum -> int
                .IsRequired();

            builder.Property(n => n.Title)
                .HasColumnName("title")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(n => n.Message)
                .HasColumnName("message")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(n => n.MetadataJson)
                .HasColumnName("metadata_json")
                .HasColumnType("json")
                .IsRequired(false);

            builder.Property(n => n.IsRead)
                .HasColumnName("is_read")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(n => n.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(n => n.ReadAt)
                .HasColumnName("read_at")
                .IsRequired(false);

            // Indexes
            builder.HasIndex(n => n.IsRead).HasDatabaseName("idx_notification_is_read");
            builder.HasIndex(n => n.Type).HasDatabaseName("idx_notification_type");
            builder.HasIndex(n => n.CreatedAt).HasDatabaseName("idx_notification_created_at");
        }
    }
}
