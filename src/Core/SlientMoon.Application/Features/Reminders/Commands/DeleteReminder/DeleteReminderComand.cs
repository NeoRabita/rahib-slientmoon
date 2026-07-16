using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Reminders.Commands.DeleteReminder
{
    public class DeleteReminderCommand : ICommand<bool>
    {
        public string Id { get; }
        public string AuthorizationHeader { get; }

        public DeleteReminderCommand(string id, string authorizationHeader)
        {
            Id = id;
            AuthorizationHeader = authorizationHeader;
        }
    }

    public class DeleteReminderCommandHandler : ICommandHandler<DeleteReminderCommand, bool>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<DeleteReminderCommandHandler> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public DeleteReminderCommandHandler(
            IUow uow,
            IAppLogger<DeleteReminderCommandHandler> logger,
            IJwtTokenService jwtTokenService)
        {
            _uow = uow;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<bool>> Handle(DeleteReminderCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.AuthorizationHeader) || !command.AuthorizationHeader.StartsWith("Bearer "))
            {
                return UserErrors.Unauthorized();
            }

            var rawToken = command.AuthorizationHeader.Replace("Bearer ", "").Trim();
            var firstQuoteIndex = rawToken.IndexOf('"');
            var token = firstQuoteIndex >= 0 ? rawToken.Substring(0, firstQuoteIndex) : rawToken;

            string userId;
            try
            {
                userId = _jwtTokenService.GetUserIdFromToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Xatırlatma silinərkən token oxunmadı: {Error}", ex.Message);
                return UserErrors.Unauthorized();
            }

            if (string.IsNullOrEmpty(userId))
            {
                return UserErrors.Unauthorized();
            }

            var reminder = await _uow.ReminderRepository.GetByIdAndUserIdAsync(command.Id, userId);

            if (reminder == null)
            {
                return ReminderErrors.NotFound(command.Id);
            }

            _logger.LogInformation("UserId {UserId} üçün Id-si {ReminderId} olan xatırlatma silinir.", userId, command.Id);

            _uow.ReminderRepository.RemoveReminder(reminder);

            return true;
        }
    }
}