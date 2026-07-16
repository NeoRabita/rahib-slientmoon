using SlientMoon.Domain.Common;
using System.Collections.Generic;

namespace SlientMoon.Domain.Entities
{
    public class Topic : BaseEntity
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string IconKey { get; set; }
        public string ColorHex { get; set; }

        public ICollection<UserTopic> UserTopics { get; set; } = new List<UserTopic>();

    }
}
