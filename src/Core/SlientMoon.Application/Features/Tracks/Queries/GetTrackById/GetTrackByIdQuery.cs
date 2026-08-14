using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Tracks;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Tracks.Queries.GetTrackById
{
    public class GetTrackByIdQuery : IQuery<TrackDetailDto>
    {
        public string Id { get; set; }

        public GetTrackByIdQuery(string id)
        {
            Id = id;
        }
    }

    public class GetTrackByIdQueryHandler : IQueryHandler<GetTrackByIdQuery, TrackDetailDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetTrackByIdQueryHandler> _logger;

        public GetTrackByIdQueryHandler(IUow uow, IAppLogger<GetTrackByIdQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<TrackDetailDto>> Handle(GetTrackByIdQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetTrackById started. TrackId: {TrackId}", query.Id);

            var track = await _uow.GenericRepository<Track>()
                .GetQueryable()
                .AsNoTracking()
                .Include(t => t.Narrator)
                .FirstOrDefaultAsync(t => t.Id == query.Id, ct);

            if (track is null)
            {
                _logger.LogWarning("GetTrackById failed. Track not found: {TrackId}", query.Id);
                return Error.NotFound("Track.NotFound", "Track not found");
            }

            var dto = new TrackDetailDto
            {
                Id = track.Id,
                CourseId = track.CourseId,
                Title = track.Title,
                Narrator = track.Narrator?.Gender.ToString().ToLower() ?? string.Empty,
                DurationSec = track.DurationSec,
                AudioUrl = track.AudioUrl,
                MimeType = track.MimeType,
                FileSizeBytes = track.FileSizeBytes,
                ImageUrl = track.ImageUrl,
                TrackNumber = track.TrackNumber
            };

            return dto;
        }
    }

}
