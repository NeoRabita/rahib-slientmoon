using SlientMoon.Domain.Enums;
using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Messages
{
    public class NotificationMessage
    {
        public NotificationType Type { get; set; }
        public string To { get; set; }
        public string? Title { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string>? ExtraData { get; set; } // Reminder push ucun lazimdir niye? 
    }
}
