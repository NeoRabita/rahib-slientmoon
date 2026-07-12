using SlientMoon.Domain.Entities;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        public string GenerateAccessToken(ApplicationUser user);

        public string GenerateRefreshToken();

        public string GetUserIdFromToken(string token);
    }
}
