using SlientMoon.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetAllAsQueryable();

        Task<Category?> GetByIdAsync(string id);
    }
}
