using FluentValidation;

namespace SlientMoon.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .EmailAddress().WithMessage("{PropertyName} must be a valid email address.");

            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage("OTP code is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MinimumLength(8).WithMessage("{PropertyName} must be at least 8 characters.");
        }
    }
}