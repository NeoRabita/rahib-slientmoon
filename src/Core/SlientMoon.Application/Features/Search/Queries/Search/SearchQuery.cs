using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.DTOs.Search;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Search.Queries.Search
{
    public record SearchQuery(
        string Q,
        int Page = 1,
        int Limit = 20
        ) : IQuery<SearchResponseDto>
    {
    }

    public class SearchQueryHandler : IQueryHandler<SearchQuery, SearchResponseDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<SearchQueryHandler> _logger;

        public SearchQueryHandler(IUow uow, IAppLogger<SearchQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<SearchResponseDto>> Handle(SearchQuery query, CancellationToken ct)
        {
            _logger.LogInformation("Universal search started. Query: {Q}", query.Q);

            var searchTerm = query.Q.Trim().ToLower();
            var searchResults = new List<SearchResultItemDto>();

            var courses = await _uow.GenericRepository<Course>()
                            .GetQueryable()
                            .AsNoTracking()
                            .Include(c => c.Category)
                                .ThenInclude(cat => cat.CategoryType)
                            .Include(c => c.CourseNarrators)
                                .ThenInclude(cn => cn.Narrator)
                            .Where(c => c.Title.ToLower().Contains(searchTerm) ||
                                        (c.Subtitle != null && c.Subtitle.ToLower().Contains(searchTerm)) ||
                                        (c.Description != null && c.Description.ToLower().Contains(searchTerm)))
                            .ToListAsync(ct);

            searchResults.AddRange(courses.Select(c => new SearchResultItemDto
            {
                Id = c.Id,
                Title = c.Title,
                Subtitle = c.Subtitle,
                Type = c.Category?.CategoryType?.Slug?.ToLower(),
                CategoryId = c.CategoryId,
                ImageUrl = c.ImageUrl,
                DurationSec = c.DurationSec,
                IsFeatured = c.IsFeatured,
                Narrators = c.CourseNarrators
                    .Where(cn => cn.Narrator != null)
                    .Select(cn => cn.Narrator!.Gender.ToString().ToLower())
                    .Distinct()
                    .ToList()
            }));

            var categories = await _uow.GenericRepository<Category>()
                .GetQueryable()
                .AsNoTracking()
                .Include(c => c.CategoryType)
                .Where(c => c.Name.ToLower().Contains(searchTerm))
                .ToListAsync(ct);

            searchResults.AddRange(categories.Select(c => new SearchResultItemDto
            {
                Id = c.Id,
                Title = c.Name,
                Subtitle = c.CategoryType?.Name,
                Type = "category",
                ImageUrl = c.IconUrl
            }));

            var topics = await _uow.GenericRepository<Topic>()
                .GetQueryable()
                .AsNoTracking()
                .Where(t => t.Title.ToLower().Contains(searchTerm))
                .ToListAsync(ct);

            searchResults.AddRange(topics.Select(t => new SearchResultItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Type = "topic"
            }));

            var tracks = await _uow.GenericRepository<Track>()
                .GetQueryable()
                .AsNoTracking()
                .Include(t => t.Narrator)
                .Where(t => t.Title.ToLower().Contains(searchTerm))
                .ToListAsync(ct);

            searchResults.AddRange(tracks.Select(t => new SearchResultItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Subtitle = t.Narrator?.Name,
                Type = "track",
                CourseId = t.CourseId,
                DurationSec = t.DurationSec,
                AudioUrl = t.AudioUrl,
                ImageUrl = t.ImageUrl
            }));

            var reminders = await _uow.GenericRepository<Reminder>()
                .GetQueryable()
                .AsNoTracking()
                .Where(r => r.Label != null && r.Label.ToLower().Contains(searchTerm))
                .ToListAsync(ct);

            searchResults.AddRange(reminders.Select(r => new SearchResultItemDto
            {
                Id = r.Id,
                Title = r.Label ?? "Reminder",
                Subtitle = r.Time.ToString(@"hh\:mm"),
                Type = "reminder",
                ReminderTime = r.Time.ToString(@"hh\:mm")
            }));


            var totalItems = searchResults.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / query.Limit);

            var pagedData = searchResults
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .ToList();

            return new SearchResponseDto
            {
                Query = query.Q,
                Data = pagedData,
                Meta = new PageMeta
                {
                    Page = query.Page,
                    Limit = query.Limit,
                    Total = totalItems,
                    TotalPages = totalPages
                }
            };
        }
    }
    }

