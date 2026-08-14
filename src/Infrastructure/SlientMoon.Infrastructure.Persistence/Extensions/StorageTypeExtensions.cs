using SlientMoon.Domain.Enums;
using SlientMoon.Infrastructure.Persistence.Configurations;

namespace SlientMoon.Infrastructure.Persistence.Extensions
{
    public static class StorageTypeExtensions
    {
        public static string GetBucketName(this StorageType storageType, MinioOptions options)
        {
            return storageType switch
            {
                StorageType.Image => options.ImageBucket,
                StorageType.Audio => options.AudioBucket,
                _ => null
            };
        }
    }
}
