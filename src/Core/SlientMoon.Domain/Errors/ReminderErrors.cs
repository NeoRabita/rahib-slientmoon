namespace SlientMoon.Domain.Errors
{
    public static class ReminderErrors
    {
        public static Error NotFound(string reminderId) => Error.NotFound(
            "Reminder.NotFound",
            $"The reminder with the Id = '{reminderId}' was not found or access denied.");
    }
}
