using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class DailyThoughtRepository : IDailyThoughtRepository
    {
        private readonly AppDbContext _context;

        public DailyThoughtRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DailyThought?> GetDailyThoughtByDateAsync(DateTime date, CancellationToken cancellationToken)
        {
            return await _context.DailyThoughts
                .AsNoTracking()
                .Include(dt => dt.Course)
                .FirstOrDefaultAsync(dt => dt.Date == date, cancellationToken);
        }
    }
}