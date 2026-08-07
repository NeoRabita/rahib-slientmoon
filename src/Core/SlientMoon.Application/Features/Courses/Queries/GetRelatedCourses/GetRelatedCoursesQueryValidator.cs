using FluentValidation;

namespace SlientMoon.Application.Features.Courses.Queries.GetRelatedCourses
{
    public class GetRelatedCoursesQueryValidator : AbstractValidator<GetRelatedCoursesQuery>
    {
        public GetRelatedCoursesQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Kurs ID-si boş ola bilməz.");

            RuleFor(x => x.Limit)
                .GreaterThan(0)
                .WithMessage("Limit 0-dan böyük olmalıdır.");
        }
    }
}
