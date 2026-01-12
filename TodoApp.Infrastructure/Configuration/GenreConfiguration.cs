using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.ToTable("Genres");

            builder.HasKey(g => g.IdGenre);

            builder.Property(g => g.NameGenre)
                   .IsRequired()
                   .HasMaxLength(100);
            
            builder.Property(g => g.IdGenre)
                   .ValueGeneratedOnAdd();
        }
    }
}