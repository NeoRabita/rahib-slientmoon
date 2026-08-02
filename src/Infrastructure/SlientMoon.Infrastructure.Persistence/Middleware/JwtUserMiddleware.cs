using Microsoft.AspNetCore.Http;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Infrastructure.Persistence.Services;
using System;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Middleware
{
    public class JwtUserMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtTokenService jwtTokenService, ICurrentUserService currentUserService)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var rawToken = authHeader.Substring("Bearer ".Length).Trim();
                var firstQuoteIndex = rawToken.IndexOf('"');
                var token = firstQuoteIndex >= 0 ? rawToken.Substring(0, firstQuoteIndex) : rawToken;

                var userId = jwtTokenService.GetUserIdFromToken(token);

                if (!string.IsNullOrEmpty(userId) && currentUserService is CurrentUserService service)
                {
                    service.SetUser(userId);
                }
            }

            await _next(context);
        }
    }
}
