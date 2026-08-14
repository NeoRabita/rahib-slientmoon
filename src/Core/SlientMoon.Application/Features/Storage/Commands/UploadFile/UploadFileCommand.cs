using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Storage;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Storage.Commands.UploadFile
{
    public record UploadFileCommand(
        Stream Stream, 
        string FileName, 
        string ContentType, 
        StorageType StorageType,
        string? EntityId = null
        ) : ICommand<UploadFileResponseDto>
    {
    }

    public class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, UploadFileResponseDto>
    {
        private readonly IStorageService _storageService;
        private readonly IUow _uow;

        public UploadFileCommandHandler(
            IStorageService storageService,
            IUow uow)
        {
            _storageService = storageService;
            _uow = uow;
        }

        public async Task<Result<UploadFileResponseDto>> Handle(UploadFileCommand command, CancellationToken ct)
        {
            if(command.Stream == null || command.Stream.Length == 0)
                return StorageErrors.FileEmpty;

            if (string.IsNullOrWhiteSpace(command.FileName))
                return StorageErrors.InvalidFileName;

            var uploadResult = await _storageService.UploadAsync(
                command.Stream,
                command.FileName,
                command.ContentType,
                command.StorageType,
                ct);

            if (!string.IsNullOrEmpty(command.EntityId) && command.StorageType == StorageType.Audio)
            {
                var track = await _uow.GenericRepository<Track>().GetByIdAsync(command.EntityId, ct);
                if (track == null)
                    return TrackErrors.TrackNotFound;

                var mimeType = !string.IsNullOrWhiteSpace(command.ContentType) && command.ContentType != "application/octet-stream"
                    ? command.ContentType
                    : Path.GetExtension(command.FileName).ToLower() switch
                    {
                        ".m4a" => "audio/mp4",
                        ".mp3" => "audio/mpeg",
                        ".wav" => "audio/wav",
                        ".ogg" => "audio/ogg",
                        ".aac" => "audio/aac",
                        _ => "audio/mpeg"
                    };

                track.AudioUrl = uploadResult.FileName;
                track.MimeType = mimeType;
                track.FileSizeBytes = command.Stream.Length;

                _uow.GenericRepository<Track>().Update(track);
            }

            return uploadResult;
        }
    }

}
