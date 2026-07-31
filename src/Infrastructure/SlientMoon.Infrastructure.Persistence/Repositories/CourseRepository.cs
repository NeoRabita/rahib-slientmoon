using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _appDbContext;

        public CourseRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Course> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Courses
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<List<Course>> GetHomeFeedCoursesAsync(CancellationToken cancellationToken)
        {
            return await _appDbContext.Courses
                .AsNoTracking()
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .ToListAsync(cancellationToken);
        }
    }
}
