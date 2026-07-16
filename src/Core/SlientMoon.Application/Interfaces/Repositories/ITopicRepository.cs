using SlientMoon.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface ITopicRepository
    {
        Task<List<Topic>> GetAllTopicsAsync();
        Task<List<Topic>> GetUserTopicsAsync(string userId);
        void RemoveUserTopic(IEnumerable<UserTopic> userTopics);
        Task AddUserTopicsAsync(IEnumerable<UserTopic> userTopics);
        Task<List<UserTopic>> GetUserTopicRelationsAsync(string userId);
        Task<bool> AreTopicsExistAsync(List<string> topicIds);
    }
}
