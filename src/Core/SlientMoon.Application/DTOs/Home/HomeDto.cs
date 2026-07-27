using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Home
{
    public class HomeDto
    {
        public List<HomeCourseDto> Recommended { get; set; } = new();
        public DailyThoughtDto? DailyThought { get; set; }
        public List<HomeCourseDto> FeaturedSleep { get; set; } = new();
        public List<HomeCourseDto> PopularMeditations { get; set; } = new();
    }

    public class HomeCourseDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public List<string> Narrators { get; set; }
    }

    public class DailyThoughtDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int DurationSec { get; set; }
        public string CourseId { get; set; }
    }
}
