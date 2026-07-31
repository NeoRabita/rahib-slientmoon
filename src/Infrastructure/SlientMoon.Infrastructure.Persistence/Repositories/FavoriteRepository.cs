using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System.Linq;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext _context;

        public FavoriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Favorite> GetAllAsFavorites()
        {
            return _context.Favorites.AsQueryable();
        }

        public async Task AddFavoriteAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
        }

        public async Task<Favorite> GetByIdAsync(string id)
        {
            return await _context.Favorites.FirstOrDefaultAsync(f => f.Id == id);
        }

        public void RemoveFavorite(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
        }
    }
}
