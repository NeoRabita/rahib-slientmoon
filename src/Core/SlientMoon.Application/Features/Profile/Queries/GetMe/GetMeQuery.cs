using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Profile.Queries.GetMe
{
    public class GetMeQuery : IQuery<UserDto>
    {
        public string AuthorizationHeader { get; set; }

        public GetMeQuery(string authorizationHeader)
        {
            AuthorizationHeader = authorizationHeader;
        }
    }

    public class GetMeQueryHandler : IQueryHandler<GetMeQuery, UserDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetMeQueryHandler> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public GetMeQueryHandler(IUow uow, IAppLogger<GetMeQueryHandler> logger, IJwtTokenService jwtTokenService)
        {
            _uow = uow;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }


        public async Task<Result<UserDto>> Handle(GetMeQuery query, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(query.AuthorizationHeader) || !query.AuthorizationHeader.StartsWith("Bearer "))
            {
                return UserErrors.Unauthorized();
            }

            // 1. "Bearer " sözünü silirik
            var rawToken = query.AuthorizationHeader.Replace("Bearer ", "").Trim();

            // 2. Əgər istifadəçi bütöv JSON yapışdırıbsa, tokenin bitdiyi dırnaq işarəsini (") tapırıq
            // və yalnız dırnağa qədər olan saf JWT token hissəsini kəsib götürürük!
            var firstQuoteIndex = rawToken.IndexOf('"');
            var token = firstQuoteIndex >= 0 ? rawToken.Substring(0, firstQuoteIndex) : rawToken;

            string userId;
            try
            {
                // 3. Artıq bura 100% təmizlənmiş token gedir
                userId = _jwtTokenService.GetUserIdFromToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Token oxunarkən format xətası baş verdi. Token zədəlidir.");
                return UserErrors.Unauthorized();
            }

            if (string.IsNullOrEmpty(userId))
            {
                return UserErrors.Unauthorized();
            }

            _logger.LogInformation("GetMe started. UserId: {UserId}", userId);

            var user = await _uow.UserRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("GetMe failed: user not found. UserId: {UserId}", userId);
                Guid.TryParse(userId, out Guid parsedGuid);
                return UserErrors.NotFound(parsedGuid);
            }

            _logger.LogInformation("GetMe done. UserId: {UserId}", userId);

            return new UserDto
            {
                Id = userId,
                Name = user.Name,
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
