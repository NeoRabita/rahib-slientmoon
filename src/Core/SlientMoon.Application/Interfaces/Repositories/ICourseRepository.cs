using SlientMoon.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        // cancellationToken params
        public Task<List<Course>> GetHomeFeedCoursesAsync(CancellationToken cancellationToken = default);

        public Task<Course?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}

