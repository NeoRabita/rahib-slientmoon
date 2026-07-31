using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.DTOs.Favorites;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Favorites.Queries.GetMyFavorites
{
    public class GetMyFavoritesQuery : IQuery<PagedResult<FavoriteDto>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public CategoryType? Type { get; set; }


        public GetMyFavoritesQuery()
        {

        }

        public GetMyFavoritesQuery(int page = 1, int limit = 20, CategoryType? type = null)
        {
            Page = page < 1 ? 1 : page;
            Limit = limit < 1 ? 20 : limit;
            Type = type;
        }
    }

    public class GetMyFavoritesQueryHandler : IQueryHandler<GetMyFavoritesQuery, PagedResult<FavoriteDto>>
    {
        private readonly IUow _uow;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAppLogger<GetMyFavoritesQueryHandler> _logger;

        public GetMyFavoritesQueryHandler(
            IUow uow,
            ICurrentUserService currentUserService,
            IAppLogger<GetMyFavoritesQueryHandler> logger)
        {
            _uow = uow;
            _currentUserService = currentUserService;
            _logger = logger;
        }


        public async Task<Result<PagedResult<FavoriteDto>>> Handle(GetMyFavoritesQuery query, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                return UserErrors.Unauthorized();
            }

            string userId = _currentUserService.UserId;

            _logger.LogInformation("GetMyFavorites started. UserId: {UserId}, Page: {Page}, Limit: {Limit}, Type: {Type}",
                userId, query.Page, query.Limit, query.Type?.ToString() ?? "All");

            var favoritesQuery = _uow.FavoriteRepository.GetAllAsFavorites()
                .Where(f => f.UserId == userId);

            
            if (query.Type.HasValue)
            {
                favoritesQuery = favoritesQuery.Where(f => f.Course.Type == query.Type.Value);
            }


            int total = await favoritesQuery.CountAsync();

            var items = await favoritesQuery
                .OrderByDescending(f => f.CreatedAt)
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .Select(f => new FavoriteDto
                {
                    Id = f.Id,
                    CourseId = f.CourseId,
                    CreatedAt = f.CreatedAt,
                    Course = new CourseDto
                    {
                        Id = f.Course.Id,
                        Title = f.Course.Title,
                        Subtitle = f.Course.Subtitle,
                        Type = f.Course.Type.ToString().ToLower(),
                        CategoryId = f.Course.CategoryId,
                        ImageUrl = f.Course.ImageUrl,
                        DurationSec = f.Course.DurationSec,
                        IsFeatured = f.Course.IsFeatured,
                        Narrators = f.Course.CourseNarrators
                            .Select(cn => cn.Narrator.Gender.ToString().ToLower())
                            .ToList()
                    }
                })
                .ToListAsync(ct);

            int totalPages = (int)Math.Ceiling((double)total / query.Limit);

            var pagedResult = new PagedResult<FavoriteDto>
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

            return pagedResult;
        }
    }
}
