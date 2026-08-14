using FluentValidation;
using System.Linq;

namespace SlientMoon.Application.Features.Courses.Queries.GetCourses
{
    public class GetCoursesQueryValidator : AbstractValidator<GetCoursesQuery>
    {
        private static readonly string[] AllowedSorts = { "createdat_desc", "createdat_asc", "title_asc", "popular" };

        public GetCoursesQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Sort)
                .Must(sort => string.IsNullOrEmpty(sort) || AllowedSorts.Contains(sort.ToLower()));

            RuleFor(x => x.Q)
                .MaximumLength(100);
        }
    }
}