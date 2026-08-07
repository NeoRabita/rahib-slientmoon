using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Tracks;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Courses.Queries.GetCourseTracks
{
    public record GetCourseTracksQuery(string CourseId, Gender? NarratorGender) : IQuery<List<CourseTrackDto>>;

    public class GetCourseTracksQueryHandler : IQueryHandler<GetCourseTracksQuery, List<CourseTrackDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetCourseTracksQueryHandler> _logger;

        public GetCourseTracksQueryHandler(IUow uow, IAppLogger<GetCourseTracksQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<List<CourseTrackDto>>> Handle(GetCourseTracksQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetCourseTracks started. CourseId: {CourseId}, GenderFilter: {Narrator}", query.CourseId, query.NarratorGender);


            var courseExists = await _uow.GenericRepository<Course>()
                .GetQueryable()
                .AsNoTracking()
                .AnyAsync(c => c.Id == query.CourseId, ct);


            if (!courseExists)
            {
                _logger.LogWarning("GetCourseTracks failed. Course not found: {CourseId}", query.CourseId);
                return Result.Failure<List<CourseTrackDto>>(DomainErrors.NotFound(query.CourseId));
            }

            var tracksQuery = _uow.GenericRepository<Track>()
                .GetQueryable()
                .AsNoTracking()
                .Include(t => t.Narrator)
                .Where(t => t.CourseId == query.CourseId);

            if (query.NarratorGender.HasValue)
            {
                tracksQuery = tracksQuery.Where(t => t.Narrator != null && t.Narrator.Gender == query.NarratorGender.Value);
            }

            var tracks = await tracksQuery
                .Select(t => new CourseTrackDto
                {
                    Id = t.Id,
                    CourseId = t.CourseId,
                    Title = t.Title,
                    Narrator = t.Narrator != null ? t.Narrator.Gender.ToString().ToLower() : string.Empty,
                    DurationSec = t.DurationSec,
                    AudioUrl = t.AudioUrl,
                    MimeType = t.MimeType,
                    FileSizeBytes = t.FileSizeBytes,
                    ImageUrl = t.ImageUrl ?? string.Empty,
                    TrackNumber = t.TrackNumber
                })
                .ToListAsync(ct);

            return tracks;
        }
    }

}
