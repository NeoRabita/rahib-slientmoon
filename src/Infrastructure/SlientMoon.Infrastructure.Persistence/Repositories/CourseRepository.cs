using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Course?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Category)
                    .ThenInclude(cat => cat.CategoryType)
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<List<Course>> GetHomeFeedCoursesAsync(CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Category)
                    .ThenInclude(cat => cat.CategoryType)
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .Include(c => c.Tracks)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
