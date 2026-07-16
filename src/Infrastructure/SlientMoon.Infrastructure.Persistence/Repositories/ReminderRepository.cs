using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly AppDbContext _appDbContext;

        public ReminderRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<List<Reminder>> GetUserRemindersAsync(string userId)
        {
            // app db de dbset elemeyim meselehedir yoxsa bu formada etmeyim? 
            return await _appDbContext.Set<Reminder>()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        public async Task<Reminder> GetByIdAndUserIdAsync(string id, string userId)
        {
            return await _appDbContext.Reminders
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        }

        public async Task AddReminderAsync(Reminder reminder)
        {
            await _appDbContext.Reminders.AddAsync(reminder);
        }

        public void UpdateReminderAsync(Reminder reminder)
        {
            _appDbContext.Reminders.Update(reminder);
        }

        public void RemoveReminder(Reminder reminder)
        {
            _appDbContext.Reminders.Remove(reminder);
        }

    }
}
