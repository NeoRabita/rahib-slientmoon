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
            //var userId = _currentUserService.GetUser();

            var track = await _uow.GenericRepository<Track>().GetByIdAsync(query.Id, ct);
            if (track is null || string.IsNullOrWhiteSpace(track.AudioUrl))
                return TrackErrors.TrackNotFound;

            var fileName = Path.GetFileName(track.AudioUrl);

            return await _storageService.GetStreamAsync(
                fileName,
                StorageType.Audio,
                query.RangeHeader,
                ct);
        }
    }
}
