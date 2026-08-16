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
            long? offset,
            long? length,
            CancellationToken ct)
        {
            var bucketName = storageType.GetBucketName(_options);

            // 1. Faylın metadatasını və ümumi ölçüsünü alırıq
            var statArgs = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName);

            var stat = await _minioClient.StatObjectAsync(statArgs, ct);
            long totalSize = stat.Size;

            // 2. Chunk ölçülərini təyin edirik
            const long CHUNK_SIZE = 1 * 1024 * 1024; // 1 MB
            long startOffset = offset ?? 0;
            long requestedLength = length ?? CHUNK_SIZE;

            if (startOffset + requestedLength > totalSize)
            {
                requestedLength = totalSize - startOffset;
            }

            var memoryStream = new MemoryStream();

            // 3. WithOffsetAndLength İSTİFADƏ ETMİRİK (Exception atmasın və callback dəqiq işləsin deyə)
            var getObjectArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithCallbackStream(async (stream, cancellationToken) =>
                {
                    // A. Tələb olunan offset pozisiyasına keçirik (Seek və ya Skip)
                    if (startOffset > 0 && stream.CanSeek)
                    {
                        stream.Seek(startOffset, SeekOrigin.Begin);
                    }
                    else if (startOffset > 0)
                    {
                        byte[] buffer = new byte[8192];
                        long skipped = 0;
                        while (skipped < startOffset)
                        {
                            int toRead = (int)Math.Min(buffer.Length, startOffset - skipped);
                            int read = await stream.ReadAsync(buffer, 0, toRead, cancellationToken);
                            if (read == 0) break;
                            skipped += read;
                        }
                    }

                    // B. Yalnız bizə lazım olan requestedLength (1 MB) qədər hissəni RAM-a kopyalayırıq
                    byte[] chunkBuffer = new byte[8192];
                    long bytesCopied = 0;
                    while (bytesCopied < requestedLength)
                    {
                        int toRead = (int)Math.Min(chunkBuffer.Length, requestedLength - bytesCopied);
                        int read = await stream.ReadAsync(chunkBuffer, 0, toRead, cancellationToken);
                        if (read == 0) break;

                        await memoryStream.WriteAsync(chunkBuffer, 0, read, cancellationToken);
                        bytesCopied += read;
                    }
                });

            // 4. MinIO-dan obyekti xətasız oxuyuruq
            await _minioClient.GetObjectAsync(getObjectArgs, ct);

            // 5. RAM-dakı stream-in göstəricisini başa qaytarırıq
            memoryStream.Position = 0;

            var contentType = string.IsNullOrWhiteSpace(stat.ContentType) ? "audio/mpeg" : stat.ContentType;

            return new TrackStreamDto
            {
                Stream = memoryStream,
                ContentType = contentType,
                TotalSize = totalSize,
                Offset = startOffset,
                Length = memoryStream.Length
            };
        }
    }

        }

