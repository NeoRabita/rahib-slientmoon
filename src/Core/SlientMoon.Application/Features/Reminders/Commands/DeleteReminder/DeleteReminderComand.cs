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

        public DeleteReminderCommand(string id)
        {
            Id = id;
        }
    }

    public class DeleteReminderCommandHandler : ICommandHandler<DeleteReminderCommand, bool>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<DeleteReminderCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public DeleteReminderCommandHandler(
            IUow uow,
            IAppLogger<DeleteReminderCommandHandler> logger,
            ICurrentUserService currentUserService)
        {
            _uow = uow;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeleteReminderCommand command, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized attempt to fetch user topics.");
                return UserErrors.Unauthorized();
            }

            string userId = _currentUserService.UserId;

            var reminder = await _uow.ReminderRepository.GetByIdAndUserIdAsync(command.Id, userId);

            if (reminder == null)
            {
                return ReminderErrors.NotFound(command.Id);
            }

            _logger.LogInformation("UserId {UserId} üçün Id-si {ReminderId} olan xatırlatma silinir.", userId, command.Id);

            _uow.ReminderRepository.Delete(reminder);

            return true;
        }
    }
}