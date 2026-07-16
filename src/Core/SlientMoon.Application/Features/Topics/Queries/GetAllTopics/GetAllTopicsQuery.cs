using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Topics;
using SlientMoon.Application.Interfaces.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Topics.Queries.GetAllTopics
{
    public class GetAllTopicsQuery : IQuery<List<TopicDto>>
    {
    }

    public class GetAllTopicsQueryHandler : IQueryHandler<GetAllTopicsQuery, List<TopicDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetAllTopicsQueryHandler> _logger;

        public GetAllTopicsQueryHandler(IUow uow, IAppLogger<GetAllTopicsQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<List<TopicDto>>> Handle(GetAllTopicsQuery query, CancellationToken ct)
        {
            _logger.LogInformation("Sistemdəki bütün mövzuların gətirilməsi sorğusu başladı.");

            var topics = await _uow.TopicRepository.GetAllTopicsAsync();

            var topicDtos = topics.Select(t => new TopicDto
            {
                Id = t.Id,
                Slug = t.Slug,
                Title = t.Title,
                ColorHex = t.ColorHex,
                IconKey = t.IconKey
            }).ToList();

            _logger.LogInformation("Sistemdə mövcud olan {Count} ədəd mövzu uğurla gətirildi.", topicDtos.Count);

            return topicDtos;
        }
    }
}
