using SlientMoon.Domain.Common;
using SlientMoon.Domain.Enums;
using System.Collections;
using System.Collections.Generic;

namespace SlientMoon.Domain.Entities
{
    public class Course : BaseEntity
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public int ViewCount { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }


        public ICollection<Track> Tracks { get; set; } = new List<Track>();
        public ICollection<CourseNarrator> CourseNarrators { get; set; } = new List<CourseNarrator>();

        public ICollection<DailyThought> DailyThoughts { get; set; } = new List<DailyThought>();
    }
}
