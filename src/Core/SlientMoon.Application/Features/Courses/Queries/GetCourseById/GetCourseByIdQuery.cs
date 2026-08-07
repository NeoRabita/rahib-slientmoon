using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Errors;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQuery : IQuery<CourseDto>
    {
        public string Id { get; set; }

        public GetCourseByIdQuery(string id)
        {
            Id = id;
        }
    }


    public class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetCourseByIdQueryHandler> _logger;

        public GetCourseByIdQueryHandler(IUow uow, IAppLogger<GetCourseByIdQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<CourseDto>> Handle(GetCourseByIdQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetCourseById started. CourseId: {CourseId}", query.Id);

            var course = await _uow.GenericRepository<Course>()
                .GetQueryable()
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.Tracks)
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .FirstOrDefaultAsync(c =>   c.Id == query.Id);

            if (course is null)
            {
                _logger.LogWarning("GetCourseById failed. Course not found: {CourseId}", query.Id);
                return Result.Failure<CourseDto>(DomainErrors.NotFound(query.Id));
            }

            var narratorTypes = course.CourseNarrators
                .Where(cn => cn.Narrator != null)
                .Select(cn => cn.Narrator!.Gender.ToString().ToLower())
                .Distinct()
                .ToList();

            var dto = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Subtitle = course.Subtitle,
                Type = course.Category?.CategoryType?.ToString().ToLower(),
                CategoryId = course.CategoryId,
                ImageUrl = course.ImageUrl ?? string.Empty,
                DurationSec = course.DurationSec,
                IsFeatured = course.IsFeatured,
                Narrators = narratorTypes,
                Description = course.Description ?? string.Empty,
                TrackCount = course.Tracks.Count,
                CreatedAt = course.CreatedAt
            };

            return dto;
        }
    }
}
