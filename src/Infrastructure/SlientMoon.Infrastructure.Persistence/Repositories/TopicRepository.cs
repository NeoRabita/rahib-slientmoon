using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class TopicRepository : GenericRepository<Topic>, ITopicRepository
    {
        public TopicRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<Topic>> GetUserTopicsAsync(string userId)
        {
            return await _dbContext.UserTopics
                .AsNoTracking()
                .Where(ut => ut.UserId == userId)
                .Select(ut => ut.Topic)
                .ToListAsync();
        }
        
        public async Task<List<UserTopic>> GetUserTopicRelationsAsync(string userId)
        {
            return await _dbContext.UserTopics
                .Where(ut => ut.UserId == userId)
                .ToListAsync();
        }

        public void RemoveUserTopic(IEnumerable<UserTopic> userTopics)
        {
            _dbContext.RemoveRange(userTopics);
        }

        public async Task AddUserTopicsAsync(IEnumerable<UserTopic> userTopics)
        {
            await _dbContext.UserTopics.AddRangeAsync(userTopics);
        }

        public async Task<bool> AreTopicsExistAsync(List<string> topicIds)
        {
            if (topicIds == null || !topicIds.Any()) return false;

            var existingTopicCount = await _dbContext.Topics
                .Where(t => topicIds.Contains(t.Id))
                .CountAsync();

            return existingTopicCount == topicIds.Count;
        }
    }
}
