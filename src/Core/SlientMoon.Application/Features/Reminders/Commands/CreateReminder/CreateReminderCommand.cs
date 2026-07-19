using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Reminders;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Reminders.Commands.CreateReminder
{
    public class CreateReminderCommand : ICommand<ReminderDto>
    {
        public DateTime Time { get; set; }
        public List<int> DaysOfWeek { get; set; }
        public string Label { get; set; }

    }

    public class CreateReminderCommandHandler : ICommandHandler<CreateReminderCommand, ReminderDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<CreateReminderCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public CreateReminderCommandHandler(
            IUow uow,
            IAppLogger<CreateReminderCommandHandler> logger,
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService)
        {
            _uow = uow;
            _logger = logger;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public async Task<Result<ReminderDto>> Handle(CreateReminderCommand command, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized attempt to create a reminder.");
                return UserErrors.Unauthorized();
            }

            _logger.LogInformation("Creating a new reminder for UserId: {UserId}", _currentUserService.UserId);

            var reminder = new Reminder
            {
                Id = Guid.NewGuid().ToString(),
                UserId = _currentUserService.UserId!,
                Time = command.Time,
                DaysOfWeek = command.DaysOfWeek,
                Label = command.Label,
                IsActive = true,
                CreatedAt = _dateTimeService.NowUtc
            };

            await _uow.ReminderRepository.AddReminderAsync(reminder);

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
