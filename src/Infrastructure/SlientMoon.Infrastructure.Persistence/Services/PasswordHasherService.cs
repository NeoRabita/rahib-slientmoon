using BCrypt.Net;
using SlientMoon.Application.Interfaces.Services;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password,hash);
        }
    }
}
