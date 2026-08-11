using FluentValidation;

namespace SlientMoon.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress();

            RuleFor(x => x.Otp)
                .NotEmpty()
                .NotNull()
                .Length(6)
                .Matches("^[0-9]{6}$");
        }
    }
}
