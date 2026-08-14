using SlientMoon.Domain.Common;
using SlientMoon.Domain.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SlientMoon.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? IconUrl { get; set; }
        public string CategoryTypeId { get; set; }
        public CategoryType CategoryType { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
