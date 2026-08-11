using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Storage;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Storage.Commands.UploadFile
{
    public record UploadFileCommand(Stream Stream, string FileName, string ContentType, StorageType StorageType) : ICommand<UploadFileResponseDto>
    {
    }

    public class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, UploadFileResponseDto>
    {
        private readonly IStorageService _storageService;

        public UploadFileCommandHandler(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<Result<UploadFileResponseDto>> Handle(UploadFileCommand command, CancellationToken ct)
        {
            if(command.Stream == null || command.Stream.Length == 0)
                return StorageErrors.FileEmpty;

            if (string.IsNullOrWhiteSpace(command.FileName))
                return StorageErrors.InvalidFileName;

            return await _storageService.UploadAsync(
                command.Stream,
                command.FileName,
                command.ContentType,
                command.StorageType,
                ct);
        }
    }

}
