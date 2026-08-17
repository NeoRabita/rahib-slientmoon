using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Storage;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Tracks.Queries.StreamTrack
{
    public record StreamTrackQuery(string Id, string? RangeHeader) : IQuery<TrackStreamDto>
    {
    }

    public class StreamTrackQueryHandler : IQueryHandler<StreamTrackQuery, TrackStreamDto>
    {
        private readonly IUow _uow;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;

        public StreamTrackQueryHandler(
            IUow uow,
            IStorageService storageService,
            ICurrentUserService currentUserService)
        {
            _uow = uow;
            _storageService = storageService;
            _currentUserService = currentUserService;
        }


        public async Task<Result<TrackStreamDto>> Handle(StreamTrackQuery query, CancellationToken ct)
        {
            var userId = _currentUserService.GetUser();

            var track = await _uow.GenericRepository<Track>().GetByIdAsync(query.Id, ct);
            if (track is null || string.IsNullOrWhiteSpace(track.AudioUrl))
                return TrackErrors.TrackNotFound;

            var fileName = Path.GetFileName(track.AudioUrl);

            long? offset = null;
            long? length = null;

            // "bytes=1024-2048" String-ini parse edirik
            if (!string.IsNullOrEmpty(query.RangeHeader) && query.RangeHeader.StartsWith("bytes="))
            {
                var rangeValue = query.RangeHeader.Replace("bytes=", "");
                var parts = rangeValue.Split('-');

                if (long.TryParse(parts[0], out long start))
                {
                    offset = start;
                    if (parts.Length > 1 && long.TryParse(parts[1], out long end))
                    {
                        length = end - start + 1;
                    }
                }
            }

            return await _storageService.GetStreamAsync(
                fileName,
                StorageType.Audio,
                offset,
                length,
                ct);
        }
    }
}
