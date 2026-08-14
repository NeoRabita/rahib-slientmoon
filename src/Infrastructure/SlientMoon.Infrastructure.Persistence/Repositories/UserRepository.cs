using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {

        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbContext.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.RefreshToken.Token == refreshToken);
        }
    }
}
