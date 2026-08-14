using FluentValidation;
using System;
using System.Linq;

namespace SlientMoon.Application.Features.Reminders.Commands.CreateReminder
{
    public class CreateReminderCommandValidator : AbstractValidator<CreateReminderCommand>
    {
        public CreateReminderCommandValidator()
        {
            RuleFor(r => r.Time)
                .NotEmpty();

            RuleFor(r => r.Label)
                .NotEmpty()
                .NotNull()
                .MaximumLength(100);

            RuleFor(r => r.DaysOfWeek)
                .NotEmpty()
                .NotNull()
                .Must(days => days.All(d => d >= 1 && d <= 7));
        }
    }
}