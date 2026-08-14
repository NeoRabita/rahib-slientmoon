using FluentValidation;

namespace SlientMoon.Application.Features.Search.Queries.Search
{
    public class SearchQueryValidator : AbstractValidator<SearchQuery>
    {
        public SearchQueryValidator()
        {
            RuleFor(x => x.Q)
                .NotEmpty()
                .MinimumLength(2);

            RuleFor(x => x.Page)
                .GreaterThan(0);

            RuleFor(x => x.Limit)
                .GreaterThan(0);
        }
    }
}
