using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class CategoryTypeConfiguration : IEntityTypeConfiguration<CategoryType>
    {
        public void Configure(EntityTypeBuilder<CategoryType> builder)
        {
            builder.ToTable("CategoryTypes");
            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Name).IsRequired().HasMaxLength(100);
            builder.Property(ct => ct.Slug).IsRequired().HasMaxLength(100);
        }
    }
}
