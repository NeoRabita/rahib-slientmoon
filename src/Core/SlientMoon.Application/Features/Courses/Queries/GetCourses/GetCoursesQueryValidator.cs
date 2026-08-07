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
                .GreaterThanOrEqualTo(1)
                .WithMessage("{PropertyName} ən azı 1 olmalıdır.");

            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 100)
                .WithMessage("{PropertyName} 1 ilə 100 arasında olmalıdır.");

            RuleFor(x => x.Sort)
                .Must(sort => string.IsNullOrEmpty(sort) || AllowedSorts.Contains(sort.ToLower()))
                .WithMessage($"{{PropertyName}} yalnız bunlardan biri ola bilər: {string.Join(", ", AllowedSorts)}.");

            RuleFor(x => x.Q)
                .MaximumLength(100)
                .WithMessage("{PropertyName} maksimum 100 simvol ola bilər.");
        }
    }
}