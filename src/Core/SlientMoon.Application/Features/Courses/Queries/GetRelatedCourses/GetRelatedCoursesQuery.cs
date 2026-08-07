using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Courses.Queries.GetRelatedCourses
{
    public record GetRelatedCoursesQuery(string Id, int Limit = 20) : IQuery<List<CourseDto>>
    {
    }

    public class GetRelatedCoursesQueryHandler : IQueryHandler<GetRelatedCoursesQuery, List<CourseDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetRelatedCoursesQueryHandler> _logger;

        public GetRelatedCoursesQueryHandler(IUow uow, IAppLogger<GetRelatedCoursesQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<List<CourseDto>>> Handle(GetRelatedCoursesQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetRelatedCourses started. CourseId: {CourseId}, Limit: {Limit}", query.Id, query.Limit);

            var currentCourse = await _uow.GenericRepository<Course>()
                .GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == query.Id, ct);

            if (currentCourse is null)
            {
                _logger.LogWarning("GetRelatedCourses failed. Course not found: {CourseId}", query.Id);
                return Result.Failure<List<CourseDto>>(DomainErrors.NotFound(query.Id));
            }

            var relatedCourses = await _uow.GenericRepository<Course>()
                .GetQueryable()
                .AsNoTracking()
                .Include(c => c.Category)
                    .ThenInclude(cat => cat.CategoryType)
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .Where(c => c.Id != query.Id && c.CategoryId == currentCourse.CategoryId)
                .Take(query.Limit)
                .ToListAsync(ct);

            if (relatedCourses.Count < query.Limit)
            {
                var remainingLimit = query.Limit - relatedCourses.Count;
                var existingIds = relatedCourses.Select(c => c.Id).ToList();
                existingIds.Add(query.Id);

                var fallbackCourses = await _uow.GenericRepository<Course>()
                    .GetQueryable()
                    .AsNoTracking()
                    .Include(c => c.Category)
                        .ThenInclude(cat => cat.CategoryType)
                    .Include(c => c.CourseNarrators)
                        .ThenInclude(cn => cn.Narrator)
                    .Where(c => !existingIds.Contains(c.Id))
                    .Take(remainingLimit)
                    .ToListAsync(ct);

                relatedCourses.AddRange(fallbackCourses);
            }

            var dtos = relatedCourses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Subtitle = c.Subtitle,
                Type = c.Category?.CategoryType?.Slug,
                CategoryId = c.CategoryId,
                ImageUrl = c.ImageUrl ?? string.Empty,
                DurationSec = c.DurationSec,
                IsFeatured = c.IsFeatured,
                Narrators = c.CourseNarrators
                    .Where(cn => cn.Narrator != null)
                    .Select(cn => cn.Narrator!.Gender.ToString().ToLower())
                    .Distinct()
                    .ToList(),
                Description = c.Description ?? string.Empty,
                CreatedAt = c.CreatedAt
            }).ToList();

            return dtos;
        }
    }
}
