using System.IO;

namespace SlientMoon.Application.DTOs.Storage
{
    public class TrackStreamDto
    {
        public Stream Stream { get; set; } = null!;
        public string ContentType { get; set; } = "audio/mpeg";
        public long TotalSize { get; set; }
        public long? Offset { get; set; }
        public long Length { get; set; }
    }
}
