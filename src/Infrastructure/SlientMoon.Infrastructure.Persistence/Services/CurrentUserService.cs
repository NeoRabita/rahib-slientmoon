using SlientMoon.Application.Interfaces.Services;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public string? UserId {  get; private set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);

        public void SetUser(string userId)
        {
            UserId = userId;
        }
    }
}
