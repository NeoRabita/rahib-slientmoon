using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.DTOs.Courses;
using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Search
{
    public class SearchResponseDto : PagedResult<SearchResultItemDto>
    {
        public string Query { get; set; }
    }

    public class SearchResultItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? CourseId { get; set; }
        public string? CategoryId { get; set; }
        public int? DurationSec { get; set; }
        public bool? IsFeatured { get; set; }
        public string? AudioUrl { get; set; }
        public string? ReminderTime { get; set; }
        public List<string>? Narrators { get; set; }
    }
}
