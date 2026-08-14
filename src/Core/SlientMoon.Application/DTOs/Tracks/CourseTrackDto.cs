namespace SlientMoon.Application.DTOs.Tracks
{
    public class CourseTrackDto
    {
        public string Id { get; set; }
        public string CourseId { get; set; }
        public string Title { get; set; }
        public string Narrator { get; set; }
        public int DurationSec { get; set; }
        public string AudioUrl { get; set; }
        public string MimeType { get; set; }
        public long FileSizeBytes { get; set; }
        public string ImageUrl { get; set; }
        public int TrackNumber { get; set; }
    }
}
