using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Reminders;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Reminders.Queries.GetMyReminders
{
    public class GetUserRemindersQuery : IQuery<List<ReminderDto>>
    {

    }

    public class GetUserRemindersQueryHandler : IQueryHandler<GetUserRemindersQuery, List<ReminderDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetUserRemindersQueryHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public GetUserRemindersQueryHandler(
            IUow uow,
            IAppLogger<GetUserRemindersQueryHandler> logger,
            ICurrentUserService currentUserService)
        {
            _uow = uow;
            _logger = logger;
            _currentUserService = currentUserService;
        }


        public async Task<Result<List<ReminderDto>>> Handle(GetUserRemindersQuery query, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized attempt to fetch user topics.");
                return UserErrors.Unauthorized();
            }

            string userId = _currentUserService.UserId;

            _logger.LogInformation("UserId {UserId} üçün xatırlatma siyahısı sorğulanır.", userId);

            var reminders = await _uow.ReminderRepository.GetUserRemindersAsync(userId);

            var reminderDtos = reminders.Select(r => new ReminderDto
            {
                Id = r.Id,
                Time = r.Time.ToString("HH:mm"),
                DaysOfWeek = r.DaysOfWeek,
                Label = r.Label,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt
            }).ToList();

            return reminderDtos;
        }
    }
}
