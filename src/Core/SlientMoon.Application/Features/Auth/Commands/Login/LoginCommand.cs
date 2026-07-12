using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Auth.Commands.Login
{
    public partial class LoginCommand : ICommand<AuthenticationResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthenticationResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUow _uow;
        private readonly IAppLogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IUow uow,
            IAppLogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResponse>> Handle(LoginCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Login started. Email: {Email}", command.Email);

            var user = await _userRepository.GetByEmailAsync(command.Email);
            
            if(user is null)
            {
                _logger.LogWarning("Login failed. User not found. Email: {Email}", command.Email);
                return UserErrors.InvalidCredentials;
            }

            if (!user.EmailVerified)
            {
                _logger.LogWarning("Login failed. Email not verified. Email: {Email}", command.Email);
                return UserErrors.EmailNotVerified;
            }

            var isPassValid = _passwordHasher.Verify(command.Password, user.PasswordHash);
            
            if (!isPassValid)
            {
                _logger.LogWarning("Login failed: invalid password. Email: {Email}", command.Email);
                return UserErrors.InvalidCredentials;
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                Expires = DateTime.UtcNow.AddDays(30),
                CreatedByIp = string.Empty,
            };

            user.RefreshToken = refreshToken;
            user.RefreshTokenId = refreshToken.Id;
            _uow.UserRepository.Update(user);

            _logger.LogInformation("Login successful. Email: {Email}", command.Email);

            return new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                TokenType = "Bearer",
                ExpiresIn = 900, // 15 minute 
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    EmailVerified = user.EmailVerified,
                    AvatarUrl = user.AvatarUrl,
                    CreatedAt = user.CreatedAt,
                }
            };
        }
    }
}
