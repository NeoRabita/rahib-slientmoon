using SlientMoon.Domain.Common;

namespace SlientMoon.Domain.Entities
{
    public class Track : BaseEntity
    {
        public string Title { get; set; }
        public int DurationSec { get; set; }
        public string AudioUrl { get; set; }
        public string MimeType { get; set; } = "audio/mpeg";
        public long FileSizeBytes { get; set; }
        public string? ImageUrl { get; set; }
        public int TrackNumber { get; set; }

        public string CourseId { get; set; } = string.Empty;
        public Course Course { get; set; } = null!;

        public string? NarratorId { get; set; }
        public Narrator? Narrator { get; set; }
    }
}
