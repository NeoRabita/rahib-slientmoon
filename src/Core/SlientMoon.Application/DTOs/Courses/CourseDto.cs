using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Courses
{
    public class CourseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public List<string> Narrators { get; set; } = new();
    }
}