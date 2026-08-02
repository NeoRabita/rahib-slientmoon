using SlientMoon.Domain.Common;

namespace SlientMoon.Domain.Entities
{
    public class Favorite : BaseEntity
    {
        public string UserId { get; set; }
        public string CourseId { get; set; }

        public ApplicationUser User { get; set; }
        public Course Course { get; set; }
    }
}
