using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Auth.Commands.Refresh
{
    public partial class RefreshCommand : ICommand<AuthenticationResponse>
    {
        public string RefreshToken { get; set; }
    }

    public class RefreshCommandHandler : ICommandHandler<RefreshCommand, AuthenticationResponse>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUow _uow;
        private readonly IAppLogger<RefreshCommandHandler> _logger;

        public RefreshCommandHandler(
            IJwtTokenService jwtTokenService,
            IUow uow,
            IAppLogger<RefreshCommandHandler> logger)
        {
            _jwtTokenService = jwtTokenService;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResponse>> Handle(RefreshCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Refresh token started.");

            var user = await _uow.UserRepository.GetByRefreshTokenAsync(command.RefreshToken);
            if (user is null)
            {
                _logger.LogWarning("Refresh failed: user not found for refresh token.");
                return UserErrors.InvalidCredentials;
            }

            if (!user.RefreshToken.IsActive)
            {
                _logger.LogWarning("Refresh failed: refresh token is not active. UserId: {UserId}", user.Id);
                return UserErrors.InvalidCredentials;
            }

            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);

            var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            user.RefreshToken.Revoked = DateTime.UtcNow;

            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenValue,
                Expires = DateTime.UtcNow.AddDays(30),
                CreatedByIp = string.Empty
            };

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenId = newRefreshToken.Id;
            _uow.UserRepository.Update(user);

            _logger.LogInformation("Refresh token successful. UserId: {UserId}", user.Id);

            return new AuthenticationResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue,
                TokenType = "Bearer",
                ExpiresIn = 900,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    EmailVerified = user.EmailVerified,
                    AvatarUrl = user.AvatarUrl,
                    CreatedAt = user.CreatedAt
                }
            };
        }
    }
}
