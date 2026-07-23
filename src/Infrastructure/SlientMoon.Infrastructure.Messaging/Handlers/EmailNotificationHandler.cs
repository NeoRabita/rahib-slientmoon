using SlientMoon.Application.DTOs.Messages;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Enums;

namespace SlientMoon.Infrastructure.Messaging.Handlers
{
    public class EmailNotificationHandler : INotificationHandler
    {
        private readonly IEmailService _emailService;

        public EmailNotificationHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public NotificationType SupportedType => NotificationType.Email;

        public async Task HandleAsync(NotificationMessage message)
        {
            await _emailService.SendOtpEmailAsync(message.To, message.Body);
        }
    }
}
