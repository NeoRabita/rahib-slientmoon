using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.Interfaces.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Courses.Queries.GetCourses
{
    public class GetCoursesQuery : IQuery<PagedResult<CourseDto>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string Sort { get; set; } = "createdAt_desc";
        public string? Type { get; set; }
        public string? CategoryId { get; set; }
        public string? Q { get; set; }
        public bool? IsFeatured { get; set; }
    }


    public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, PagedResult<CourseDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetCoursesQueryHandler> _logger;

        public GetCoursesQueryHandler(IUow uow, IAppLogger<GetCoursesQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<PagedResult<CourseDto>>> Handle(GetCoursesQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetCourses started. Page: {Page}, Limit: {Limit}, Sort: {Sort}", query.Page, query.Limit, query.Sort);

            var coursesQuery = _uow.CourseRepository.GetQueryable();


            if (!string.IsNullOrWhiteSpace(query.Type))
            {
                var normalizedType = query.Type.Trim().ToLower();
                coursesQuery = coursesQuery.Where(c => c.Category.CategoryType.Slug.ToLower() == normalizedType);
            }

            if (!string.IsNullOrWhiteSpace(query.CategoryId))
            {
                coursesQuery = coursesQuery.Where(c => c.CategoryId == query.CategoryId);
            }

            if (query.IsFeatured.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.IsFeatured == query.IsFeatured.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Q))
            {
                var search = query.Q.Trim().ToLower();
                coursesQuery = coursesQuery.Where(c => c.Title.ToLower().Contains(search) || c.Subtitle.ToLower().Contains(search));
            }

            int total = await coursesQuery.CountAsync(ct);

            coursesQuery = query.Sort?.ToLower() switch
            {
                "createdat_asc" => coursesQuery.OrderBy(c => c.CreatedAt),
                "title_asc" => coursesQuery.OrderBy(c => c.Title),
                "popular" => coursesQuery.OrderByDescending(c => c.ViewCount),
                _ => coursesQuery.OrderByDescending(c => c.CreatedAt)
            };

            var items = await coursesQuery
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Subtitle = c.Subtitle,
                    Type = c.Category.CategoryType.Slug.ToLower(),
                    CategoryId = c.CategoryId,
                    ImageUrl = c.ImageUrl,
                    DurationSec = c.DurationSec,
                    IsFeatured = c.IsFeatured,
                    Narrators = c.CourseNarrators
                        .Select(cn => cn.Narrator.Gender.ToString().ToLower())
                        .ToList()
                })
                .AsNoTracking()
                .ToListAsync(ct);

            int totalPages = (int)Math.Ceiling((double)total / query.Limit);

            _logger.LogInformation("GetCourses completed. Returned {Count} items out of {Total}.", items.Count, total);

            return new PagedResult<CourseDto>
            {
                Data = items,
                Meta = new PageMeta
                {
                    Page = query.Page,
                    Limit = query.Limit,
                    Total = total,
                    TotalPages = totalPages
                }
            };
        }
    }
}
