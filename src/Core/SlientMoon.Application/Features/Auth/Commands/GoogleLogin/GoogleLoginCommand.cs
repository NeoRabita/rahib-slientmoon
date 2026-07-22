using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Auth.Commands.GoogleLogin
{
    public partial class GoogleLoginCommand : ICommand<AuthenticationResponse>
    {
        public string IdToken { get; set; }
    }

    public class GoogleLoginCommandHandler : ICommandHandler<GoogleLoginCommand, AuthenticationResponse>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAppLogger<GoogleLoginCommandHandler> _logger;
        private readonly IUow _uow;

        public GoogleLoginCommandHandler(
            IGoogleAuthService googleAuthService,
            IAppLogger<GoogleLoginCommandHandler> logger,
            IJwtTokenService jwtTokenService,
            IUow uow)
        {
            _googleAuthService = googleAuthService;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _uow = uow;
        }

        public async Task<Result<AuthenticationResponse>> Handle(GoogleLoginCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Google Login started. IdToken: {IdToken}", command.IdToken);


            var googleResult = await _googleAuthService.VerifyTokenAsync(command.IdToken);

            if (googleResult.IsFailure)
            {
                _logger.LogWarning("Google token verification failed.");
                return Result.Failure<AuthenticationResponse>(googleResult.Error);
            }

            var googleUser = googleResult.Value;

            var user = await _uow.UserRepository.GetByEmailAsync(googleUser.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = googleUser.Email,
                    Name = googleUser.Name,
                    AvatarUrl = googleUser.AvatarUrl,
                    EmailVerified = true,
                    LoginType = LoginType.Google,
                    CreatedAt = DateTime.UtcNow
                };

                await _uow.UserRepository.AddAsync(user);
            } else
            {
                if (user.LoginType == LoginType.Normal)
                {
                    user.LoginType = LoginType.Google;
                }

                if (!string.IsNullOrEmpty(googleUser.AvatarUrl))
                {
                    user.AvatarUrl = googleUser.AvatarUrl;
                }
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            user.RefreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                Expires = DateTime.UtcNow.AddDays(30),
                Created = DateTime.UtcNow
            };

            _uow.UserRepository.Update(user);


            var response = new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                TokenType = "Bearer",
                ExpiresIn = 900,
                User = new UserDto
                {
                    Id = user.Id.ToString(),
                    Name = user.Name,
                    Email = user.Email,
                    EmailVerified = user.EmailVerified,
                    AvatarUrl = user.AvatarUrl,
                    CreatedAt = user.CreatedAt
                }
            };

            return Result.Success(response);
        }
}
}
