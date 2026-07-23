using SlientMoon.Application.DTOs.Messages;
using SlientMoon.Domain.Enums;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface INotificationHandler
    {
        NotificationType SupportedType { get; }
        Task HandleAsync(NotificationMessage message);
    }
}
