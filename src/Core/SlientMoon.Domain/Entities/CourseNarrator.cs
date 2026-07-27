namespace SlientMoon.Domain.Entities
{
    public class CourseNarrator
    {
        public string CourseId { get; set; }
        public Course Course { get; set; }

        public string NarratorId { get; set; }
        public Narrator Narrator { get; set; }
    }
}
