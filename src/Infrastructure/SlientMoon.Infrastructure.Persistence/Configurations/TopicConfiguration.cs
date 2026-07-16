using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;
using System;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.ToTable("Topics");

            builder.HasKey(t => t.Id);

            builder.HasData(
                new Topic
                {
                    Id = "6f9b17f4-d55c-4f7f-a123-1d54bdf19001",
                    Slug = "reduce-stress",
                    Title = "Reduce Stress",
                    IconKey = "leaf",
                    ColorHex = "#8E97FD",
                    CreatedAt = DateTime.UtcNow
                },
                new Topic
                {
                    Id = "6f9b17f4-d55c-4f7f-a123-1d54bdf19002",
                    Slug = "improve-sleep",
                    Title = "Improve Sleep",
                    IconKey = "moon",
                    ColorHex = "#FFC97E",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
