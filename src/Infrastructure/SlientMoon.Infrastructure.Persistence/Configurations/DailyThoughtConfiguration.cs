using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class DailyThoughtConfiguration : IEntityTypeConfiguration<DailyThought>
    {
        public void Configure(EntityTypeBuilder<DailyThought> builder)
        {
            builder.HasKey(dt => dt.Id);

            builder.HasOne(dt => dt.Course)
                   .WithMany(c => c.DailyThoughts)
                   .HasForeignKey(dt => dt.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}