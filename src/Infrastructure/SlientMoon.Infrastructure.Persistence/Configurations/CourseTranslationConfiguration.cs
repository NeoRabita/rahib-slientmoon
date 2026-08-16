using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class CourseTranslationConfiguration : IEntityTypeConfiguration<CourseTranslation>
    {
        public void Configure(EntityTypeBuilder<CourseTranslation> builder)
        {
            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.LanguageCode)
                .IsRequired()
                .HasMaxLength(10); // "az", "en", "ru" üçün yetərlidir

            builder.Property(ct => ct.Title)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(ct => ct.Subtitle)
                .HasMaxLength(500);

            // Bir kursun eyni dildə 2 tərcüməsi ola bilməz (Unique Index)
            builder.HasIndex(ct => new { ct.CourseId, ct.LanguageCode })
                .IsUnique();

            // Cascade Delete: Kurs silinəndə tərcümələri də silinsin
            builder.HasOne(ct => ct.Course)
                .WithMany(c => c.Translations)
                .HasForeignKey(ct => ct.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
