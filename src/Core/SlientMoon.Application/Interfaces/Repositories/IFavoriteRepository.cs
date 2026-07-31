using SlientMoon.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface IFavoriteRepository
    {
        IQueryable<Favorite> GetAllAsFavorites();
        Task AddFavoriteAsync(Favorite favorite);
        Task<Favorite?> GetByIdAsync(string id);
        void RemoveFavorite(Favorite favorite);
    }
}
