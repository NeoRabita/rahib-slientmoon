using System;
using System.Threading;
using System.Threading.Tasks;
using SlientMoon.Domain.Entities;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface IDailyThoughtRepository
    {
        Task<DailyThought?> GetDailyThoughtByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}