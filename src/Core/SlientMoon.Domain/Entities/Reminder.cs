using SlientMoon.Domain.Common;
using System;
using System.Collections.Generic;

namespace SlientMoon.Domain.Entities
{
    public class Reminder : BaseEntity
    {
        public string UserId { get; set; }
        public DateTime Time { get; set; }
        public List<int> DaysOfWeek { get; set; }
        public string Label { get; set; }
        public bool IsActive { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
