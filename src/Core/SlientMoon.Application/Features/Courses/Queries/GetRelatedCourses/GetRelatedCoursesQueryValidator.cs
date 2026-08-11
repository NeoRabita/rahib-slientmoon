using FluentValidation;

namespace SlientMoon.Application.Features.Courses.Queries.GetRelatedCourses
{
    public class GetRelatedCoursesQueryValidator : AbstractValidator<GetRelatedCoursesQuery>
    {
        public GetRelatedCoursesQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Limit)
                .GreaterThan(0);
        }
    }
}
