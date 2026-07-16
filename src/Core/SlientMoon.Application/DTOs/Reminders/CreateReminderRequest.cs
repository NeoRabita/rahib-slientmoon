using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Reminders
{
    public class CreateReminderRequest
    {
        public string Time { get; set; }
        public List<int> DaysOfWeek { get; set; }
        public string Label { get; set; }
    }
}
