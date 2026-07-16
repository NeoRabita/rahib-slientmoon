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
        public string AuthorizationHeader { get; }

        public GetUserRemindersQuery(string authorizationHeader)
        {
            AuthorizationHeader = authorizationHeader;
        }
    }

    public class GetUserRemindersQueryHandler : IQueryHandler<GetUserRemindersQuery, List<ReminderDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetUserRemindersQueryHandler> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public GetUserRemindersQueryHandler(
            IUow uow,
            IAppLogger<GetUserRemindersQueryHandler> logger,
            IJwtTokenService jwtTokenService)
        {
            _uow = uow;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }


        public async Task<Result<List<ReminderDto>>> Handle(GetUserRemindersQuery query, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(query.AuthorizationHeader) || !query.AuthorizationHeader.StartsWith("Bearer "))
            {
                return UserErrors.Unauthorized();
            }

            var rawToken = query.AuthorizationHeader.Replace("Bearer ", "").Trim();
            var firstQuoteIndex = rawToken.IndexOf('"');
            var token = firstQuoteIndex >= 0 ? rawToken.Substring(0, firstQuoteIndex) : rawToken;

            string userId;


            try
            {
                userId = _jwtTokenService.GetUserIdFromToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Xatırlatmalar gətirilərkən token oxunmadı: {Error}", ex.Message);
                return UserErrors.Unauthorized();
            }

            if (string.IsNullOrEmpty(userId))
            {
                return UserErrors.Unauthorized();
            }

            _logger.LogInformation("UserId {UserId} üçün xatırlatma siyahısı sorğulanır.", userId);

            
            var reminders = await _uow.ReminderRepository.GetUserRemindersAsync(userId);

            var reminderDtos = reminders.Select(r => new ReminderDto
            {
                Id = r.Id,
                Time = r.Time,
                DaysOfWeek = r.DaysOfWeek,
                Label = r.Label,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt
            }).ToList();

            return reminderDtos;
        }
    }
}
