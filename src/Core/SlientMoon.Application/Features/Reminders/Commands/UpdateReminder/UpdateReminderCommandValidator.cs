using FluentValidation;
using System.Linq;

namespace SlientMoon.Application.Features.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
    {
        public UpdateReminderCommandValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .NotNull();

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