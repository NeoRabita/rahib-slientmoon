using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System.Linq;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }


        public IQueryable<Category> GetAllAsQueryable()
        {
            return _context.Categories.AsQueryable();
        }

        public async Task<Category> GetByIdAsync(string id)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(category => category.Id == id);
        }
    }
}
