using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SlientMoon.Application;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Infrastructure.Messaging;
using SlientMoon.Infrastructure.Persistence;
using SlientMoon.Infrastructure.Persistence.Contexts;
using SlientMoon.Infrastructure.Persistence.Middleware;
using SlientMoon.Infrastructure.Persistence.Seed;
using SlientMoon.WebApi.Extensions;
using System.Text.Json.Serialization;

namespace SlientMoon.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllForDev", builder =>
                {
                    builder
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            services.DisableDefaultApiValidation();
            services.AddControllers()
                .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
            services.AddHttpContextAccessor();
            services.AddApplicationLayer();
            services.AddPersistenceRegistration(Configuration);
            services.AddPersistenceApiServices(Configuration);
            services.AddMessagingServices();
            services.AddSwaggerExtension();
            services.AddLocalization();
            services.AddServiceExtension();
            services.EnableApiVersioning();
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseErrorHandling(env);
            app.UseLocalization();
            //app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAllForDev");
            app.UseSwaggerExtension(env, provider);
            app.UseAuthentication();
            app.UseMiddleware<JwtUserMiddleware>();
            app.UseAuthorization();

            SeedDatabase(app);
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void SeedDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    var dateTimeService = services.GetRequiredService<IDateTimeService>();

                    DbInitializer.SeedAsync(context, dateTimeService).GetAwaiter().GetResult();
                }
                catch (System.Exception ex)
                {
                    var logger = services.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Startup>>();
                    logger.LogError(ex, "Verilənlər bazasına Seed Data doldurularkən xəta baş verdi.");
                }
            }
        }
    }
}
