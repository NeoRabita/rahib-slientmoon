using System.IO;

namespace SlientMoon.Application.DTOs.Storage
{
    public class TrackStreamDto
    {
        public Stream Stream { get; set; } = null!;
        public string ContentType { get; set; } = "audio/mpeg";
        public int StatusCode { get; set; } = 200;
        public long? ContentLength { get; set; }
        public string? ContentRange { get; set; }
    }
}
