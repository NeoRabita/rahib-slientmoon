using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Topics;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Topics.Commands.UpdateUserTopics
{
    public class UpdateUserTopicsCommand : ICommand<List<TopicDto>>
    {
        public string AuthorizationHeader { get; }
        public List<string> TopicIds { get; }

        public UpdateUserTopicsCommand(string authorizationHeader, List<string> topicIds)
        {
            AuthorizationHeader = authorizationHeader;
            TopicIds = topicIds;
        }
    }

    public class UpdateUserTopicCommandHandler : ICommandHandler<UpdateUserTopicsCommand, List<TopicDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<UpdateUserTopicCommandHandler> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public UpdateUserTopicCommandHandler(
            IUow uow,
            IAppLogger<UpdateUserTopicCommandHandler> logger,
            IJwtTokenService jwtTokenService)
        {
            _uow = uow;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<List<TopicDto>>> Handle(UpdateUserTopicsCommand command, CancellationToken ct)
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
                _logger.LogWarning("Mövzu yeniləmə uğursuz oldu: Token format xətası. Ətraflı: {Error}", ex.Message);
                return UserErrors.Unauthorized();
            }

            if (string.IsNullOrEmpty(userId))
            {
                return UserErrors.Unauthorized();
            }

            _logger.LogInformation("UserId {UserId} üçün mövzu yeniləmə prosesi başladı.", userId);

 
            bool areTopicsValid = await _uow.TopicRepository.AreTopicsExistAsync(command.TopicIds);

            if (!areTopicsValid)
            {
                _logger.LogWarning("UserId {UserId} üçün mövzu yenilənməsi uğursuz oldu: Göndərilən bəzi ID-lər bazada tapılmadı.", userId);
                return Error.NotFound("Topic.NotFound", "Seçilmiş mövzulardan biri və ya bir neçəsi tapılmadı.");
            }

            var existingRelations = await _uow.TopicRepository.GetUserTopicRelationsAsync(userId);

            if (existingRelations.Any())
            {
                _logger.LogInformation("UserId {UserId} üçün {Count} ədəd köhnə mövzu əlaqəsi silinir.", userId, existingRelations.Count);
                _uow.TopicRepository.RemoveUserTopic(existingRelations);
            }

            var newRelations = command.TopicIds.Select(topicId => new UserTopic
            {
                UserId = userId,
                TopicId = topicId
            }).ToList();

            await _uow.TopicRepository.AddUserTopicsAsync(newRelations);

            await _uow.SaveChangesAsync(ct);
            var updatedTopics = await _uow.TopicRepository.GetAllTopicsAsync();
            var userTopicsList = updatedTopics
                .Where(t => command.TopicIds.Contains(t.Id))
                .Select(t => new TopicDto
                {
                    Id = t.Id,
                    Slug = t.Slug,
                    Title = t.Title,
                    ColorHex = t.ColorHex,
                    IconKey = t.IconKey
                }).ToList();

            _logger.LogInformation("UserId {UserId} üçün mövzular uğurla yeniləndi. Yeni mövzu sayı: {Count}", userId, userTopicsList.Count);

            return userTopicsList;
        }
    }
}