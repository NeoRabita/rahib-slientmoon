using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class CourseNarratorConfiguration : IEntityTypeConfiguration<CourseNarrator>
    {
        public void Configure(EntityTypeBuilder<CourseNarrator> builder)
        {
            builder.HasKey(cn => new { cn.CourseId, cn.NarratorId });

            builder.HasOne(cn => cn.Course)
                   .WithMany(c => c.CourseNarrators)
                   .HasForeignKey(cn => cn.CourseId);

            builder.HasOne(cn => cn.Narrator)
                   .WithMany(n => n.CourseNarrators)
                   .HasForeignKey(cn => cn.NarratorId);
        }
    }
}