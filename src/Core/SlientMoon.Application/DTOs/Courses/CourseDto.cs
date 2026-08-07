using System;
using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Courses
{
    public class CourseDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Type { get; set; }
        public string CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public List<string> Narrators { get; set; } = new();

        public string? Description { get; set; }
        public int TrackCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}