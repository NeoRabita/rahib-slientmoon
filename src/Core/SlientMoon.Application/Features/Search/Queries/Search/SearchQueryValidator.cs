using FluentValidation;

namespace SlientMoon.Application.Features.Search.Queries.Search
{
    public class SearchQueryValidator : AbstractValidator<SearchQuery>
    {
        public SearchQueryValidator()
        {
            RuleFor(x => x.Q)
                .NotEmpty()
                .WithMessage("Axtarış ifadəsi boş ola bilməz.")
                .MinimumLength(2)
                .WithMessage("Axtarış ifadəsi minimum 2 simvol olmalıdır.");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page 0-dan böyük olmalıdır.");

            RuleFor(x => x.Limit)
                .GreaterThan(0)
                .WithMessage("Limit 0-dan böyük olmalıdır.");
        }
    }
}
