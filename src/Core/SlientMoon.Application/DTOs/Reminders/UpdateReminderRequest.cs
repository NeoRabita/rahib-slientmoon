using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Reminders
{
    public class UpdateReminderRequest
    {
        public string Time { get; set; }
        public List<int> DaysOfWeek { get; set; }
        public string Label { get; set; }
        public bool IsActive { get; set; }
    }
}
