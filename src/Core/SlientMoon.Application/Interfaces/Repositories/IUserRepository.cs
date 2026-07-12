using SlientMoon.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken);
        Task AddAsync(ApplicationUser user);
        void Update(ApplicationUser user);
    }
}
