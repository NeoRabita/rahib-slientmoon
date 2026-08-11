using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SlientMoon.Application.DTOs.Storage;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Enums;
using SlientMoon.Infrastructure.Persistence.Configurations;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SlientMoon.Infrastructure.Persistence.Extensions;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class MinioStorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioOptions _options;

        public MinioStorageService(IMinioClient minioClient, IOptions<MinioOptions> options)
        {
            _minioClient = minioClient;
            _options = options.Value;
        }

        public async Task<UploadFileResponseDto> UploadAsync(Stream fileStream, string fileName, string contentType, StorageType storageType, CancellationToken cancellationToken)
        {
            var bucketName = storageType.GetBucketName(_options);
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(uniqueFileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            var protocol = _options.SSL ? "https" : "http";
            var url = $"{protocol}://{_options.Endpoint}/{bucketName}/{uniqueFileName}";
            
            return new UploadFileResponseDto
            {
                FileName = uniqueFileName,
                Url = url,
                Message = "File uploaded successfully."
            };


        }
        public async Task<Stream> DownloadAsync(string fileName, StorageType storageType, CancellationToken cancellationToken)
        {
            var bucketName = storageType.GetBucketName(_options);
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));

            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
            memoryStream.Position = 0;

            return memoryStream;
        }
        public async Task DeleteAsync(string fileName, StorageType storageType, CancellationToken cancellationToken)
        {
            var bucketName = storageType.GetBucketName(_options);

            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs,cancellationToken);
        }

    }
}
