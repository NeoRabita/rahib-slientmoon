using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.Interfaces.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQuery : IQuery<List<CategoryDto>>
    {
        public string? Type { get; set; }

        public GetCategoriesQuery(string? type = null)
        {
            Type = type;
        }
    }

    public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetCategoriesQueryHandler> _logger;

        public GetCategoriesQueryHandler(IUow uow, IAppLogger<GetCategoriesQueryHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetCategories started. Filter Type: {Type}", query.Type?.ToString() ?? "All");

            var categoryQuery = _uow.CategoryRepository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(query.Type))
            {
                var normalizedType = query.Type.Trim().ToLower();
                categoryQuery = categoryQuery.Where(c => c.CategoryType.Slug.ToLower() == normalizedType);
            }

            var categories = await categoryQuery
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Title = c.Name,
                    Type = c.CategoryType.Name,
                    CategoryTypeId = c.CategoryTypeId,
                    IconUrl = c.IconUrl
                })
                .ToListAsync(ct);

            _logger.LogInformation("GetCategories completed. Count: {Count}", categories.Count);

            return categories;
        }
    }

}
