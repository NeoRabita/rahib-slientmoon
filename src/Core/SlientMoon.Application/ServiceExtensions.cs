using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SlientMoon.Application.Behaviours.Localization;
using SlientMoon.Application.Interfaces.Messaging;
using System.Reflection;

namespace SlientMoon.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<IDispatcher, Dispatcher>();
            services.AddCqrsHandlers(Assembly.GetExecutingAssembly());
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LocalizationBehavior<,>));
        }
    }
}