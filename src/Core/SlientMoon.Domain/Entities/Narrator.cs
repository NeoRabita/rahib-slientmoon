using SlientMoon.Domain.Common;
using SlientMoon.Domain.Enums;
using System.Collections;
using System.Collections.Generic;

namespace SlientMoon.Domain.Entities
{
    public class Narrator : BaseEntity
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }

        public ICollection<CourseNarrator> CourseNarrators { get; set; } = new List<CourseNarrator>();
    }
}
