using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQuery : IQuery<List<CategoryDto>>
    {
        public CategoryType? Type { get; set; }

        public GetCategoriesQuery(CategoryType? type = null)
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

            var categoryQuery = _uow.CategoryRepository.GetAllAsQueryable();

            if (query.Type.HasValue)
            {
                categoryQuery = categoryQuery.Where(c => c.CategoryType == query.Type.Value);
            }

            var categories = await categoryQuery
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Title = c.Name,
                    Type = c.CategoryType.ToString().ToLower(),
                    IconUrl = c.IconUrl
                })
                .ToListAsync(ct);

            _logger.LogInformation("GetCategories completed. Count: {Count}", categories.Count);

            return categories;
        }
    }

}
