using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class ReminderRepository : GenericRepository<Reminder>, IReminderRepository
    {
        public ReminderRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
        public async Task<List<Reminder>> GetUserRemindersAsync(string userId)
        {
            // app db de dbset elemeyim meselehedir yoxsa bu formada etmeyim? 
            return await _dbSet
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        public async Task<Reminder> GetByIdAndUserIdAsync(string id, string userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        }
    }
}
