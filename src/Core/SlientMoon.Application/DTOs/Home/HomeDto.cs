using SlientMoon.Application.DTOs.Courses;
using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Home
{
    public class HomeDto
    {
        public List<CourseDto> Recommended { get; set; } = new();
        public DailyThoughtDto? DailyThought { get; set; }
        public List<CourseDto> FeaturedSleep { get; set; } = new();
        public List<CourseDto> PopularMeditations { get; set; } = new();
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
