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
    public class GetUserTopicsQuery : IQuery<List<TopicDto>>
    {
        public string AuthorizationHeader { get; set; }
        public GetUserTopicsQuery(string authorizationHeader)
        {
            AuthorizationHeader = authorizationHeader;
        }
    }

    public class GetUserTopicsQueryHandler : IQueryHandler<GetUserTopicsQuery, List<TopicDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetUserTopicsQueryHandler> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public GetUserTopicsQueryHandler(IUow uow, IAppLogger<GetUserTopicsQueryHandler> logger, IJwtTokenService jwtTokenService)
        {
            _uow = uow;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<List<TopicDto>>> Handle(GetUserTopicsQuery query, CancellationToken ct)
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
                _logger.LogWarning("Get user topics failed: Token format error. Details: {Error}", ex.Message);
                return UserErrors.Unauthorized();
            }

            if (string.IsNullOrEmpty(userId))
            {
                return UserErrors.Unauthorized();
            }

            _logger.LogInformation("UserId {UserId} üçün seçilmiş mövzuların gətirilməsi sorğusu başladı.", userId);

            var topics = await _uow.TopicRepository.GetUserTopicsAsync(userId);

            var result = topics.Select(t => new TopicDto
            {
                Id = t.Id,
                Slug = t.Slug,
                Title = t.Title,
                ColorHex = t.ColorHex,
                IconKey = t.IconKey
            }).ToList();

            _logger.LogInformation("UserId {UserId} üçün {Count} ədəd seçilmiş mövzu uğurla gətirildi.", userId, result.Count);

            return result;
        }
    }
}