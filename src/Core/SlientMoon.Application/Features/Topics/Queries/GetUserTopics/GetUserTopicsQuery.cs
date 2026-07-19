using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Topics;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Topics.Queries.GetUserTopics
{
    public record GetUserTopicsQuery : IQuery<List<TopicDto>>;

    public class GetUserTopicsQueryHandler : IQueryHandler<GetUserTopicsQuery, List<TopicDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetUserTopicsQueryHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public GetUserTopicsQueryHandler(IUow uow, IAppLogger<GetUserTopicsQueryHandler> logger, ICurrentUserService currentUserService)
        {
            _uow = uow;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<TopicDto>>> Handle(GetUserTopicsQuery query, CancellationToken ct)
        {

            if (!_currentUserService.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized attempt to fetch user topics.");
                return UserErrors.Unauthorized();
            }

            string userId = _currentUserService.UserId;

            var userTopics = await _uow.TopicRepository.GetUserTopicsAsync(userId);

            var result = userTopics.Select(t => new TopicDto
            {
                Id = t.Id,
                Slug = t.Slug,
                Title = t.Title,
                ColorHex = t.ColorHex,
                IconKey = t.IconKey
            }).ToList();

            _logger.LogInformation("UserId {UserId} üçün {Count} ədəd seçilmiş mövzu uğurla gətirildi.", userId, result.Count);

            return Result.Success(result);
        }
    }
}