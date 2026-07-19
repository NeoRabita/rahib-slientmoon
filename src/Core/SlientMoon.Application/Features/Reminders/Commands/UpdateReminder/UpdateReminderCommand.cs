using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Reminders;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderCommand : ICommand<ReminderDto>
    {
        public string Id { get; }
        public DateTime Time { get; }
        public List<int> DaysOfWeek { get; }
        public string Label { get; }
        public bool IsActive { get; }

        public UpdateReminderCommand(string id, UpdateReminderRequest request)
        {
            Id = id;
            Time = request.Time;
            DaysOfWeek = request.DaysOfWeek;
            Label = request.Label;
            IsActive = request.IsActive;
        }

        public class UpdateReminderCommandHandler : ICommandHandler<UpdateReminderCommand, ReminderDto>
        {
            private readonly IUow _uow;
            private readonly IAppLogger<UpdateReminderCommandHandler> _logger;
            private readonly ICurrentUserService _currentUserService;
            private readonly IDateTimeService _dateTimeService;

            public UpdateReminderCommandHandler(
                IUow uow,
                IAppLogger<UpdateReminderCommandHandler> logger,
                ICurrentUserService currentUserService,
                IDateTimeService dateTimeService)
            {
                _uow = uow;
                _logger = logger;
                _currentUserService = currentUserService;
                _dateTimeService = dateTimeService;
            }

            public async Task<Result<ReminderDto>> Handle(UpdateReminderCommand command, CancellationToken cancellationToken)
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

                _logger.LogInformation("UserId {UserId} üçün Id-si {ReminderId} olan xatırlatma yenilənir.", userId, command.Id);

                reminder.Time = command.Time;
                reminder.DaysOfWeek = command.DaysOfWeek;
                reminder.Label = command.Label;
                reminder.IsActive = command.IsActive;

                _uow.ReminderRepository.UpdateReminderAsync(reminder);

                var reminderDto = new ReminderDto
                {
                    Id = reminder.Id,
                    Time = reminder.Time.ToString("HH:mm"),
                    DaysOfWeek = reminder.DaysOfWeek,
                    Label = reminder.Label,
                    IsActive = reminder.IsActive,
                    CreatedAt = reminder.CreatedAt
                };

                return reminderDto;
            }
        }
    }
}