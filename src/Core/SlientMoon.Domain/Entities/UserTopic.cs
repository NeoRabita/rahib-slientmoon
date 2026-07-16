namespace SlientMoon.Domain.Entities
{
    public class UserTopic
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }


        public string TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
