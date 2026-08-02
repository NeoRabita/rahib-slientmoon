using SlientMoon.Application.DTOs.Courses;
using System;

namespace SlientMoon.Application.DTOs.Favorites
{
    public class FavoriteDto
    {
        public string Id { get; set; }
        public string CourseId { get; set; }
        public CourseDto Course { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
