using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.DTOs.Home;
using SlientMoon.Domain.Entities;
using System.Linq;

namespace SlientMoon.Application.Mappings
{
    public static class HomeFeedMappingExtensions
    {
        public static CourseDto ToHomeCourseDto(this Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Subtitle = course.Subtitle,
                Type = course.Category?.CategoryType?.Slug?.ToLower() ?? string.Empty,
                ImageUrl = course.ImageUrl,
                DurationSec = course.DurationSec,
                IsFeatured = course.IsFeatured,
                Narrators = course.CourseNarrators
                    .Select(cn => cn.Narrator.Gender.ToString().ToLower())
                    .ToList()
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