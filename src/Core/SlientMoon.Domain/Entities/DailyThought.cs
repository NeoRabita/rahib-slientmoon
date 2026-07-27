using SlientMoon.Domain.Common;
using System;

namespace SlientMoon.Domain.Entities
{
    public class DailyThought : BaseEntity
    {
        public DateTime Date { get; set; }

        public string CourseId { get; set; }
        public Course Course { get; set; } 
    }
}
