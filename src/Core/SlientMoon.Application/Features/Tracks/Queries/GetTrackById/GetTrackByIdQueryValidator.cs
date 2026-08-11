using FluentValidation;

namespace SlientMoon.Application.Features.Tracks.Queries.GetTrackById
{
    public class GetTrackByIdQueryValidator : AbstractValidator<GetTrackByIdQuery>
    {
        public GetTrackByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
