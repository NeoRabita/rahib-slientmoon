using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
    {
        public void Configure(EntityTypeBuilder<Translation> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasIndex(t => new { t.Key, t.LanguageCode })
                   .IsUnique();

            builder.Property(t => t.Key)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(t => t.Value)
                   .IsRequired();
 
            builder.Property(t => t.LanguageCode)
                   .HasConversion<string>()
                   .HasMaxLength(10)
                   .IsRequired();
        }
    }
}
