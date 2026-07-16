using SlientMoon.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface IReminderRepository
    {
        Task<List<Reminder>> GetUserRemindersAsync(string userId);

        Task<Reminder?> GetByIdAndUserIdAsync(string id, string userId);

        Task AddReminderAsync(Reminder reminder);

        void UpdateReminderAsync(Reminder reminder);

        void RemoveReminder(Reminder reminder);
    }
}
