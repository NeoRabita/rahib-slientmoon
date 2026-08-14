namespace SlientMoon.Infrastructure.Persistence.Configurations
{
    public class MinioOptions
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public bool SSL { get; set; }
        public string ImageBucket { get; set; }
        public string AudioBucket { get; set; }
    }
}
