using SlientMoon.Application.DTOs.Storage;
using SlientMoon.Domain.Enums;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<UploadFileResponseDto> UploadAsync(Stream fileStream, string fileName, string contentType, StorageType storageType, CancellationToken cancellationToken);
        Task<Stream> DownloadAsync(string fileName, StorageType storageType, CancellationToken cancellationToken);
        Task DeleteAsync(string fileName, StorageType storageType, CancellationToken cancellationToken);
    }
}

