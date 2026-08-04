using Microsoft.AspNetCore.Http;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using SlientMoon.Infrastructure.Persistence.Services;
using System;
using System.Text.Json;
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

        public async Task InvokeAsync(
            HttpContext context,
            IJwtTokenService jwtTokenService,
            ICurrentUserService currentUserService)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                await ReturnUnauthorized(context, "Token tapılmadı");
                return;
            }

            var rawToken = authHeader.Substring("Bearer ".Length).Trim();
            var firstQuoteIndex = rawToken.IndexOf('"');
            var token = firstQuoteIndex >= 0 ? rawToken.Substring(0, firstQuoteIndex) : rawToken;

            var userId = jwtTokenService.GetUserIdFromToken(token);

            if (string.IsNullOrEmpty(userId))
            {
                await ReturnUnauthorized(context, "Token etibarsızdır");
                return;
            }

            if (currentUserService is CurrentUserService service)
            {
                service.SetUser(userId);
            }

            await _next(context);
        }

        private static async Task ReturnUnauthorized(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var error = new
            {
                code = "Users.Unauthorized",
                description = message ?? "İcazə verilməyib. Zəhmət olmasa daxil olun."
            };

            var json = JsonSerializer.Serialize(error);
            await context.Response.WriteAsync(json);
        }
    }
}
