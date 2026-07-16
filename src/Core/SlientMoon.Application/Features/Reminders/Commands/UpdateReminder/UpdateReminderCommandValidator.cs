using FluentValidation;
using System.Linq;

namespace SlientMoon.Application.Features.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
    {
        public UpdateReminderCommandValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(r => r.Time)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .Matches(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")
                .WithMessage("{PropertyName} must be in HH:mm format (e.g., 07:30).");

            RuleFor(r => r.Label)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");

            RuleFor(r => r.DaysOfWeek)
                .NotEmpty().WithMessage("At least one day must be selected.")
                .NotNull()
                .Must(days => days.All(d => d >= 1 && d <= 7))
                .WithMessage("Days must be between 1 (Monday) and 7 (Sunday).");
        }
    }
}