using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Infrastructure.Persistence.Contexts;

namespace SlientMoon.Infrastructure.Persistence.Seed
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context, IDateTimeService dateTimeService)
        {
            // Baza artıq doludursa təkrar işləməsin
            if (await context.Courses.AnyAsync()) return;

            // Hazırkı vaxtı öz servisindən alırıq
            var now = dateTimeService.NowUtc;
            var today = now.Date;

            // 1. Kategoriyalar (YENİ SƏHƏLƏR ƏLAVƏ OLUNDU)
            var sleepCategory = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Sleep",
                CategoryType = CategoryType.Sleep,
                Slug = "sleep",
                IconUrl = "https://cdn.silentmoon.app/icons/sleep.png",
                CreatedAt = now
            };

            var meditationCategory = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Meditation",
                CategoryType = CategoryType.Meditation,
                Slug = "meditation",
                IconUrl = "https://cdn.silentmoon.app/icons/meditation.png",
                CreatedAt = now
            };

            var anxiousCategory = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Anxious",
                CategoryType = CategoryType.Meditation, // və ya uyğun gördüyün tip
                Slug = "anxious",
                IconUrl = "https://cdn.silentmoon.app/icons/anxious.png",
                CreatedAt = now
            };

            await context.Categories.AddRangeAsync(sleepCategory, meditationCategory, anxiousCategory);

            // 2. Səsləndirənlər (Narrators)
            var maleNarrator = new Narrator { Id = Guid.NewGuid().ToString(), Name = "Male Voice", Gender = Gender.Male, CreatedAt = now };
            var femaleNarrator = new Narrator { Id = Guid.NewGuid().ToString(), Name = "Female Voice", Gender = Gender.Female, CreatedAt = now };

            await context.Narrators.AddRangeAsync(maleNarrator, femaleNarrator);

            // 3. Kurslar (Courses)
            var course1 = new Course
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Night Island",
                Subtitle = "Sleep Music",
                Type = CategoryType.Meditation,
                ImageUrl = "",
                DurationSec = 1800,
                IsFeatured = true,
                ViewCount = 150,
                CategoryId = sleepCategory.Id,
                CreatedAt = now
            };

            var course2 = new Course
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Sweet Dreams",
                Subtitle = "Deep Sleep",
                Type = CategoryType.Sleep,
                ImageUrl = "",
                DurationSec = 2400,
                IsFeatured = true,
                ViewCount = 90,
                CategoryId = sleepCategory.Id,
                CreatedAt = now
            };

            var course3 = new Course
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Morning Calmness",
                Subtitle = "Focus & Relax",
                Type = CategoryType.Meditation,
                ImageUrl = "",
                DurationSec = 600,
                IsFeatured = false,
                ViewCount = 300,
                CategoryId = meditationCategory.Id,
                CreatedAt = now
            };

            await context.Courses.AddRangeAsync(course1, course2, course3);

            // 4. Course - Narrator Əlaqələri
            var courseNarrators = new List<CourseNarrator>
            {
                new CourseNarrator { CourseId = course1.Id, NarratorId = maleNarrator.Id },
                new CourseNarrator { CourseId = course1.Id, NarratorId = femaleNarrator.Id },
                new CourseNarrator { CourseId = course2.Id, NarratorId = femaleNarrator.Id },
                new CourseNarrator { CourseId = course3.Id, NarratorId = maleNarrator.Id }
            };

            await context.CourseNarrators.AddRangeAsync(courseNarrators);

            // 5. Daily Thought
            var dailyThought = new DailyThought
            {
                Id = Guid.NewGuid().ToString(),
                Date = today,
                CourseId = course1.Id,
                CreatedAt = now
            };

            await context.DailyThoughts.AddAsync(dailyThought);

            await context.SaveChangesAsync();
        }
    }
}