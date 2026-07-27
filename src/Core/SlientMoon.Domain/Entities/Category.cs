using SlientMoon.Domain.Common;
using System.Collections;
using System.Collections.Generic;

namespace SlientMoon.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        // new List<Course>(); sual

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
