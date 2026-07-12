using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Auth.Commands.Logout
{
    public partial class LogoutCommand : ICommand<bool>
    {
        public string RefreshToken { get; set; }
    }

    public class LogoutCommandHandler : ICommandHandler<LogoutCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUow _uow;
        private readonly IAppLogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IUserRepository userRepository,
            IUow uow,
            IAppLogger<LogoutCommandHandler> logger)
        {
            _userRepository = userRepository;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(LogoutCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Logout started.");

            var user = await _userRepository.GetByRefreshTokenAsync(command.RefreshToken);

            if(user is null)
            {
                _logger.LogWarning("Logout failed: user not found for refresh token.");
                return UserErrors.InvalidCredentials;
            }

            if (!user.RefreshToken.IsActive)
            {
                _logger.LogWarning("Logout failed: token already revoked. UserId: {UserId}", user.Id);
                return UserErrors.InvalidCredentials;
            }

            user.RefreshToken.Revoked = DateTime.UtcNow;

            _uow.UserRepository.Update(user);

            _logger.LogInformation("Logout successful. UserId: {UserId}", user.Id);

            return true;
        }
    }

}
