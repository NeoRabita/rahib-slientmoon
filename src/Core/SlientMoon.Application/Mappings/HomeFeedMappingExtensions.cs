using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.DTOs.Home;
using SlientMoon.Domain.Entities;
using System.Linq;

namespace SlientMoon.Application.Mappings
{
    public static class HomeFeedMappingExtensions
    {
        public static CourseDto ToHomeCourseDto(this Course course, string currentLang)
        {
            var translation = course.Translations?.FirstOrDefault(t => t.LanguageCode == currentLang);

            return new CourseDto
            {
                Id = course.Id,
                Title = translation?.Title ?? course.Title,
                Subtitle = translation?.Subtitle ?? course.Subtitle,
                Description = translation?.Description ?? course.Description,
                Type = course.Category?.CategoryType?.Slug?.ToLower() ?? string.Empty,
                ImageUrl = course.ImageUrl,
                DurationSec = course.DurationSec,
                IsFeatured = course.IsFeatured,
                CreatedAt = course.CreatedAt,
                CategoryId = course.CategoryId,
                TrackCount = course.Tracks != null ? course.Tracks.Count() : 0,
                Narrators = course.CourseNarrators != null
                    ? course.CourseNarrators
                        .Where(cn => cn.Narrator != null)
                        .Select(cn => cn.Narrator.Gender.ToString().ToLower())
                        .ToList() : new()
            };
        }

        public static DailyThoughtDto ToDailyThoughtDto(this DailyThought dailyThought)
        {
            return new DailyThoughtDto
            {
                Id = dailyThought.Id,
                Title = dailyThought.Course.Title,
                Subtitle = dailyThought.Course.Subtitle,
                DurationSec = dailyThought.Course.DurationSec,
                CourseId = dailyThought.CourseId
            };
        }
    }
}