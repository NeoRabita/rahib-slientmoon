using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SlientMoon.Application;
using SlientMoon.Infrastructure.Messaging;
using SlientMoon.Infrastructure.Persistence;
using SlientMoon.Infrastructure.Persistence.Middleware;
using SlientMoon.WebApi.Extensions;

namespace SlientMoon.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.DisableDefaultApiValidation();
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddApplicationLayer();
            services.AddPersistenceRegistration(Configuration);
            services.AddPersistenceApiServices(Configuration);
            services.AddMessagingServices();
            services.AddSwaggerExtension();
            services.AddLocalization();
            services.AddServiceExtension();
            services.EnableApiVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseErrorHandling(env);
            app.UseLocalization();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSwaggerExtension(env, provider);
            app.UseAuthentication();
            app.UseMiddleware<JwtUserMiddleware>();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
