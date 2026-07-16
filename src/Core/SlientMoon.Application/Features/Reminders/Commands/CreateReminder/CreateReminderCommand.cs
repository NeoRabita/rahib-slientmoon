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
        public string AuthorizationHeader { get; }
        public string Time { get; }
        public List<int> DaysOfWeek { get; }
        public string Label { get; }

        public CreateReminderCommand(string authorizationHeader, CreateReminderRequest request)
        {
            AuthorizationHeader = authorizationHeader;
            Time = request.Time;
            DaysOfWeek = request.DaysOfWeek;
            Label = request.Label;
        }
    }

    public class CreateReminderCommandHandler : ICommandHandler<CreateReminderCommand, ReminderDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<CreateReminderCommandHandler> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public CreateReminderCommandHandler(
            IUow uow,
            IAppLogger<CreateReminderCommandHandler> logger,
            IJwtTokenService jwtTokenService)
        {
            _uow = uow;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<ReminderDto>> Handle(CreateReminderCommand command, CancellationToken ct)
        {
            // her defe validasiya edirem bunu ayri bir funksiyaya cixardim? 

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
                _logger.LogWarning("Xatırlatma yaradılarkən token oxunmadı: {Error}", ex.Message);
                return UserErrors.Unauthorized();
            }

            if (string.IsNullOrEmpty(userId))
            {
                return UserErrors.Unauthorized();
            }

            _logger.LogInformation("UserId {UserId} üçün yeni xatırlatma yaradılır.", userId);

            // 2. Yeni Reminder entity-sinin formalaşdırılması
            var reminder = new Reminder
            {
                Id = Guid.NewGuid().ToString(), 
                UserId = userId,
                Time = command.Time,
                DaysOfWeek = command.DaysOfWeek,
                Label = command.Label,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.ReminderRepository.AddReminderAsync(reminder);

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
