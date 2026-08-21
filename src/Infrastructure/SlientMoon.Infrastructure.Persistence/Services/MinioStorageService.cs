using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SlientMoon.Application.DTOs.Storage;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Enums;
using SlientMoon.Infrastructure.Persistence.Configurations;
using SlientMoon.Infrastructure.Persistence.Extensions;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class MinioStorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinioOptions _options;
        private static readonly HttpClient _httpClient = new();

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

        public async Task<TrackStreamDto> GetStreamAsync(
            string fileName,
            StorageType storageType,
            string? rangeHeader,
            CancellationToken ct)
        {
            var bucketName = storageType.GetBucketName(_options);

            var presignedArgs = new PresignedGetObjectArgs()
                           .WithBucket(bucketName)
                           .WithObject(fileName)
                           .WithExpiry(60 * 60);

            var presignedUrl = await _minioClient.PresignedGetObjectAsync(presignedArgs);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, presignedUrl);

            // 3. Client-dən gələn Range header-ini MinIO-ya yönləndiririk
            if (!string.IsNullOrEmpty(rangeHeader))
            {
                requestMessage.Headers.TryAddWithoutValidation("Range", rangeHeader);
            }

            // 4. Sorğunu göndəririk (Body-ni buffering etmədən, yalnız header-ləri oxuyuruq)
            var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, ct);

            var responseStream = await response.Content.ReadAsStreamAsync(ct);

            return new TrackStreamDto
            {
                Stream = responseStream,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "audio/mpeg",
                StatusCode = (int)response.StatusCode,
                ContentLength = response.Content.Headers.ContentLength,
                ContentRange = response.Content.Headers.ContentRange?.ToString()
            };
        }
    }

        }

