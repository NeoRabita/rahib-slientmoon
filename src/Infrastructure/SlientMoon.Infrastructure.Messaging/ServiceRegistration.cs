using Microsoft.Extensions.DependencyInjection;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Infrastructure.Messaging.Consumers;
using SlientMoon.Infrastructure.Messaging.Handlers;
using SlientMoon.Infrastructure.Messaging.Services;

namespace SlientMoon.Infrastructure.Messaging
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddMessagingServices(this IServiceCollection services)
        {
            services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

            services.AddScoped<INotificationHandler, EmailNotificationHandler>();

            services.AddHostedService<NotificationConsumer>();

            return services;
        }
    }
}
