using SlientMoon.Domain.Common;
using System.Collections.Generic;

namespace SlientMoon.Domain.Entities
{
    public class CategoryType : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
