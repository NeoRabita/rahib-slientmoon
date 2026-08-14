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
        public List<string> TopicIds { get; }

        public UpdateUserTopicsCommand(List<string> topicIds)
        {
            TopicIds = topicIds;
        }
    }

    public class UpdateUserTopicCommandHandler : ICommandHandler<UpdateUserTopicsCommand, List<TopicDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<UpdateUserTopicCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserTopicCommandHandler(
            IUow uow,
            IAppLogger<UpdateUserTopicCommandHandler> logger,
            ICurrentUserService currentUserService)
        {
            _uow = uow;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<TopicDto>>> Handle(UpdateUserTopicsCommand command, CancellationToken ct)
        {

            string userId = _currentUserService.GetUser();

            _logger.LogInformation("UserId {UserId} üçün mövzu yeniləmə prosesi başladı.", userId);

            bool areTopicsValid = await _uow.TopicRepository.AreTopicsExistAsync(command.TopicIds);

            if (!areTopicsValid)
            {
                _logger.LogWarning("UserId {UserId} üçün mövzu yenilənməsi uğursuz oldu: Göndərilən bəzi ID-lər bazada tapılmadı.", userId);
                return Error.NotFound("Topic.NotFound", "Seçilmiş mövzulardan biri və ya bir neçəsi tapılmadı.");
            }

            var existingRelations = await _uow.TopicRepository.GetUserTopicRelationsAsync(userId);
            var existingTopicIds = existingRelations.Select(r => r.TopicId).ToList();

            var relationsToRemove = existingRelations
                .Where(r => !command.TopicIds.Contains(r.TopicId))
                .ToList();

            var topicIdsToAdd = command.TopicIds
                .Where(id => !existingTopicIds.Contains(id))
                .ToList();

            if (relationsToRemove.Any())
            {
                _logger.LogInformation("Removing {Count} obsolete topic relations for UserId: {UserId}", relationsToRemove.Count, userId);
                _uow.TopicRepository.RemoveUserTopic(relationsToRemove);
            }

            if (topicIdsToAdd.Any())
            {
                _logger.LogInformation("Adding {Count} new topic relations for UserId: {UserId}", topicIdsToAdd.Count, userId);
                var newRelations = topicIdsToAdd.Select(topicId => new UserTopic
                {
                    UserId = userId,
                    TopicId = topicId
                }).ToList();

                await _uow.TopicRepository.AddUserTopicsAsync(newRelations);
            }

            var updatedTopics = await _uow.TopicRepository.GetAllAsync();
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

            _logger.LogInformation("Topics successfully updated for UserId: {UserId}. Total topics: {Count}", userId, userTopicsList.Count);

            return userTopicsList;
        }
    }
}