using SlientMoon.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        public Task<List<Course>> GetHomeFeedCoursesAsync(CancellationToken cancellationToken = default);
    }
}

