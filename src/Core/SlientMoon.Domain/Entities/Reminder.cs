using SlientMoon.Domain.Common;
using System.Collections.Generic;

namespace SlientMoon.Domain.Entities
{
    public class Reminder : BaseEntity
    {
        public string UserId { get; set; }
        // string olmalidir yoxsa basqa type? 
        public string Time { get; set; }
        public List<int> DaysOfWeek { get; set; }
        public string Label { get; set; }
        public bool IsActive { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
