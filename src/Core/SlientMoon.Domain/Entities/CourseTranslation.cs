using SlientMoon.Domain.Common;

namespace SlientMoon.Domain.Entities
{
    public class CourseTranslation : BaseEntity
    {
        public string CourseId { get; set; }
        public Course Course { get; set; }
        public string LanguageCode { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
    }
}
