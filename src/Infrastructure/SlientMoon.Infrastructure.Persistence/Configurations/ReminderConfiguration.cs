using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SlientMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
    {
        public void Configure(EntityTypeBuilder<Reminder> builder)
        {
            builder.ToTable("Reminders");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.Property(r => r.Time)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(r => r.Label)
                .IsRequired()
                .HasMaxLength(100);

            
            builder.Property(r => r.DaysOfWeek)
                .HasConversion(
                    v => string.Join(',', v),
                    v => string.IsNullOrEmpty(v)
                        ? new List<int>()
                        : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                )
                .IsRequired();


            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
