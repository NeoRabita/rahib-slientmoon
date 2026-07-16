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
        public string AuthorizationHeader { get; }
        public string Time { get; }
        public List<int> DaysOfWeek { get; }
        public string Label { get; }
        public bool IsActive { get; }

        public UpdateReminderCommand(string id, string authorizationHeader, UpdateReminderRequest request)
        {
            Id = id;
            AuthorizationHeader = authorizationHeader;
            Time = request.Time;
            DaysOfWeek = request.DaysOfWeek;
            Label = request.Label;
            IsActive = request.IsActive;
        }

        public class UpdateReminderCommandHandler : ICommandHandler<UpdateReminderCommand, ReminderDto>
        {
            private readonly IUow _uow;
            private readonly IAppLogger<UpdateReminderCommandHandler> _logger;
            private readonly IJwtTokenService _jwtTokenService;

            public UpdateReminderCommandHandler(
                IUow uow,
                IAppLogger<UpdateReminderCommandHandler> logger,
                IJwtTokenService jwtTokenService)
            {
                _uow = uow;
                _logger = logger;
                _jwtTokenService = jwtTokenService;
            }

            public async Task<Result<ReminderDto>> Handle(UpdateReminderCommand command, CancellationToken cancellationToken)
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
                    _logger.LogWarning("Xatırlatma yenilənərkən token oxunmadı: {Error}", ex.Message);
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

                _logger.LogInformation("UserId {UserId} üçün Id-si {ReminderId} olan xatırlatma yenilənir.", userId, command.Id);

                reminder.Time = command.Time;
                reminder.DaysOfWeek = command.DaysOfWeek;
                reminder.Label = command.Label;
                reminder.IsActive = command.IsActive;

                _uow.ReminderRepository.UpdateReminderAsync(reminder);

                var reminderDto = new ReminderDto
                {
                    Id = reminder.Id,
                    Time = reminder.Time,
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