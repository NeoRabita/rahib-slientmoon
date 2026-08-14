using FluentValidation;

namespace SlientMoon.Application.Features.Reminders.Commands.DeleteReminder
{
    public class DeleteReminderCommandValidator : AbstractValidator<DeleteReminderCommand>
    {
        public DeleteReminderCommandValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .NotNull();
        }
    }
}