using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Slug).IsRequired().HasMaxLength(150);

            builder.HasOne(c => c.CategoryType)
                   .WithMany(ct => ct.Categories)
                   .HasForeignKey(c => c.CategoryTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}