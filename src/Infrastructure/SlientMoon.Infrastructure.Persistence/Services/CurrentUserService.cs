using SlientMoon.Application.Interfaces.Services;
using System;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public string? UserId { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(UserId);

        public void SetUser(string userId)
        {
            UserId = userId;
        }

        public string GetUser()
        {
            if (!IsAuthenticated || string.IsNullOrEmpty(UserId))
            {
                throw new UnauthorizedAccessException();
            }

            return UserId;
        }
    }
}