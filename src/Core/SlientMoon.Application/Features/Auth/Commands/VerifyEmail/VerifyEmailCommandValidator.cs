using FluentValidation;

namespace SlientMoon.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .EmailAddress().WithMessage("{PropertyName} must be a valid email address.");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .Length(6).WithMessage("{PropertyName} must be 6 characters.")
                .Matches("^[0-9]{6}$").WithMessage("{PropertyName} must contain only digits.");
        }
    }
}
