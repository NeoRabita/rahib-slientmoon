using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Messaging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Behaviours.Auth
{
    public class AuthBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAppLogger<AuthBehavior<TRequest, TResponse>> _logger;

        public AuthBehavior(
            ICurrentUserService currentUserService,
            IAppLogger<AuthBehavior<TRequest, TResponse>> logger)
        {
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next)
        {
            // 1. Əgər sorğu IRequireAuth tələb etmirsə, birbaşa növbəti addıma keç
            if (request is not IRequireAuth)
            {
                return await next();
            }

            // 2. Auth yoxlanışı
            if (!_currentUserService.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized access attempt to {RequestName}", typeof(TRequest).Name);

                // Əgər TResponse sizin Result<T> / Result tipinizdirsə, birbaşa UserErrors qaytarırıq
                if (typeof(TResponse).IsGenericType &&
                    typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    return (dynamic)UserErrors.Unauthorized();
                }

                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            // 3. Problem yoxdursa handler-ə ötür
            return await next();
        }
    }
}